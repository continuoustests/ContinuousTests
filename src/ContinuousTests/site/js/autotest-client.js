var pageTitle = 'AutoTest.Net';
$.at = new Object();
$.at.errors = new Object();
$.at.warnings = new Object()
$.at.failures = new Object()
$.at.ignored = new Object()
$.at.selectedItem = 0;
$.at.selectedLink = 0;
$.at.token = "";

$(document).ready(function() {

    document.title = pageTitle + ' - Connecting...';
    $('#status').text("Collecting source and project information...");

    $(window).keydown(keydown);

    if ("WebSocket" in window) {
        connect();
    } else {
        console.log("Browser does not support web sockets", 'error');
    };

    Handlebars.registerHelper('isSelected', function(item, options) {
        if(item.id === $.at.selectedItem) {
            return options.fn(this);
        }
        return options.inverse(this);
    });
});

function keydown(e) {
    var keyCode = e.keyCode;

    if (keyCode === 34) { // PgDown
        selectItem(getItemByOffset(10));
        return false;
    }
    if (keyCode === 33) { // PgUp
        selectItem(getItemByOffset(-10));
        return false;
    }
    if (keyCode === 36) { // Home
        selectItem(getFirstItem());
        return false;
    }
    if (keyCode === 35) { // End
        selectItem(getLastItem());
        return false;
    }
    // Handling keyboard shortcuts within detail field
    var details = $('#'+$.at.selectedItem+'_details');
    if (details.hasClass('listitemDetailsVisible')) {
        if (keyCode === 27) { // Esc
            exitCurrent();
            $.at.selectedLink = 0;
            $("div[name='link']").removeClass('listitemLinkSelected').addClass('listitemLink');
            $("a[name='linkHref']").removeClass('listitemLinkHrefSelected').addClass('listitemLinkHref');
            return false;
        }
        if (keyCode === 74) { // j
            selectLink(details, $.at.selectedLink + 1);
            return true;
        }
        if (keyCode === 75) { // k
            selectLink(details, $.at.selectedLink - 1);
            return false;
        }
        if (keyCode == 13) { // Enter
            activeLink(clickLink);
            return false;
        }
        return true;
    } else {
        if (keyCode === 27) { // Esc
            focusEditor();
            return false;
        }
    }
    if (keyCode === 74) { // j
        selectItem(getNextItem());
        return true;
    }
    if (keyCode === 75) { // k
        selectItem(getPreviousItem());
        return false;
    }
    if (keyCode === 73) { // i
        showInformation();    
        return false;
    }
    return true;
}

function openInEditor(file, line, column) {
    send('goto', { file: file, line: line, column: column });
}

function buildAndTestAll() {
   send('build-test-all', {}); 
}

function buildAndTestProjects(projects) {
    send('build-test-projects', { projects: projects });
}

function abortRun() {
    send('abort-run', {});
}

function detectRecursionOnNextRun() {
    send('detect-recursion-on-next-run', {});
}

function pauseEngine() {
    send('engine-pause', {});   
}

function resumeEngine() {
    send('engine-resume', {});   
}

function getEngineState(response) {
    request('get-engine-state', {}, function (body) {
        response(body);
    });
}

function focusEditor() {
    send("focus-editor");
}

function send(subject, body) {
    $.at.belly.send(subject, body);
}

function request(subject, body, response) {
    $.at.belly.send(subject, body, response);
}

function connect() {

    $.at.belly = createBellyRubClient();
    $.at.belly.onconnected = function() {
        $.at.belly.request('get-token-path', {}, function (body) {
            $.at.token = body.token;
        });
    };
    $.at.belly.ondisconnected = function() {
        document.title = pageTitle + ' - Disconnected';
        // Fool firefox into thinking it can close the window
        window.open('','_parent',''); 
        window.close(); 
    };
    $.at.belly.handlers['shutdown'] = function (body) {
        client.disconnect();
    };
    window.onbeforeunload = function () {
        $.at.belly.disconnect();
    };

    $.at.belly.handlers['vm-spawned'] = function (body) {
        document.title = pageTitle + ' - ' + $.at.token;
        $('#status').text("Engine started and waitin for changes");
    };
    $.at.belly.handlers['run-started'] = function (body) {
        $('#status').text("run started...");
    };
    $.at.belly.handlers['run-finished'] = function (body) {
        console.log('run finished');
    };
    $.at.belly.handlers['status-information'] = function (body) {
        $('#status').text(body.message);
    };
    $.at.belly.handlers['picture-update'] = function (body) {
        console.log('pic: ' + body.state);
        var img = 'graphics/circleAbort.png';
        if (body.state === 'progress')
            img = 'graphics/progress.gif';
        else if (body.state === 'green')
            img = 'graphics/circleWIN.png';
        else if (body.state === 'red')
            img = 'graphics/circleFAIL.png';
        $('#status-picture').attr('src', img);
    };
    $.at.belly.handlers['add-item'] = function (body) {
        console.log(body);
        if (body.type === 'Build error')
            $.at.errors[body.id] = body;
        else if (body.type === 'Build warning')
            $.at.warnings[body.id] = body;
        else if (body.type === 'Test failed')
            $.at.failures[body.id] = body;
        else if (body.type === 'Test ignored')
            $.at.ignored[body.id] = body;
        updateList();
    };
    $.at.belly.handlers['remove-builditem'] = function (body) {
        removeListItem($.at.errors, $.at.warnings, body.id);
        updateList();
    };
    $.at.belly.handlers['remove-testitem'] = function (body) {
        removeListItem($.at.failures, $.at.ignored, body.id);
        updateList();
    };
    $.at.belly.handlers['remove-builditems'] = function (body) {
        for (var i = body.ids.length - 1; i >= 0; i--) {
            removeListItem($.at.errors, $.at.warnings, body.ids[i]);
        };
        updateList();
    };
    $.at.belly.handlers['selected-store'] = function (body) {
    };
    $.at.belly.handlers['selected-restore'] = function (body) {
    };
    $.at.belly.handlers['run-summary'] = function (body) {
        selectItem(getFirstItem());
    };
    $.at.belly.handlers['shutdown'] = function (body) {
        window.close();
    };
    $.at.belly.handlers['recursive-run-result'] = function (body) {
        console.log(body);
    };

    $.at.belly.connect(); 
};

function removeListItem(list1, list2, id) {
    if (id in list1) {
        delete list1[id];
        return;
    }
    if (id in list2) {
        delete list2[id];
        return;
    }
}

function updateList() {
    compileTemplate('#listTemplate', '#list', {
        errors: $.at.errors,
        warnings: $.at.warnings,
        failures: $.at.failures,
        ignored: $.at.ignored
    });
}

function itemClicked(id) {
    selectItem(id);
    showInformation();
}

function showInformation() {
    console.log('running show information');
    $('#'+$.at.selectedItem).removeClass('listitemSelected').addClass('listitemExpanded');
    var details = $('#'+$.at.selectedItem+'_details');
    $(details).removeClass('listitemDetailsHidden').addClass('listitemDetailsVisible');
    selectLink(details, 0);
}

function exitCurrent() {
    $('#'+$.at.selectedItem).removeClass('listitemExpanded').addClass('listitemSelected');
    $('#'+$.at.selectedItem+'_details').removeClass('listitemDetailsVisible').addClass('listitemDetailsHidden');
    focusWindow($('#'+$.at.selectedItem));
}

function selectItem(id) {
    if (id === $.at.selectedItem)
        return;
    $("div[id$='_details']").removeClass('listitemDetailsVisible').addClass('listitemDetailsHidden');
    if (id !== $.at.selectedItem) {
        $('#'+$.at.selectedItem).removeClass('listitemSelected');
        $('#'+$.at.selectedItem).removeClass('listitemExpanded');
    }
    $('#'+id).addClass('listitemSelected');
    if ($('.listitemSelected').length === 0) {
        id = getFirstItem();
        $('#'+id).addClass('listitemSelected');
    }
    console.log('Selected: '+id);
    $.at.selectedItem = id;
    exitCurrent();
    focusWindow($('#'+id));
}

function selectLink(details, indexToSelect) {
    var currentLink = $.at.selectedLink;
    var index = 0;

    $(details).children().each(function (idx, link) {
        if ($(link).attr("name") === 'link') {
            console.log('Checking link ' + index.toString() + ' ' + $(link).attr('name'));
            if (index === indexToSelect) {
                withLink(currentLink, function (current) {
                    $(current).removeClass('listitemLinkSelected').addClass('listitemLink');
                    $(current).children().each(function (idx2, avalue) {
                        $(avalue).removeClass('listitemLinkHrefSelected').addClass('listitemLinkHref');
                    });
                });
                $(link).removeClass('listitemLink').addClass('listitemLinkSelected');
                $(link).children().each(function (idx2, avalue) {
                    $(avalue).removeClass('listitemLinkHref').addClass('listitemLinkHrefSelected');
                });
                $.at.selectedLink = index;
                focusWindow(link);
            }
            index++;
        }
    });
}

function activeLink(handler) {
    withLink($.at.selectedLink, handler);
}

function withLink(currentLink, handler) {
    var details = $('#'+$.at.selectedItem+'_details');
    if (details.hasClass('listitemDetailsVisible')) {
        var index = 0;
        $(details).children().each(function (idx, link) {
            if ($(link).attr("name") === 'link') {
                if (index === currentLink) {
                    console.log('active link: ' + $(link).attr('name'));
                    handler(link);
                }
                index++;
            }
        });
    }
}

function clickLink(link) {
    $(link).children().each(function (idx2, avalue) {
        console.log('clicking link: ' + $(avalue).attr('name'));
        $(avalue).click();
    });
}

function focusWindow(obj) {
    var offset = $(obj).offset();
    if(isOnScreen(obj) === false)  {
        offset.top -= 40;
        $('html, body').animate({
            scrollTop: offset.top,
        }, 200);
    }
}

function isOnScreen(elem) {
    var $window = $(window)
    var viewport_top = $window.scrollTop()
    var viewport_height = getViewPortSize('Height');
    var viewport_bottom = viewport_top + viewport_height
    var $elem = $(elem)
    var top = $elem.offset().top
    var height = $elem.height()
    var bottom = top + height
    console.log('vewport bottom: ' + viewport_bottom.toString() + ' element bottom: ' + bottom.toString());

    return (top >= viewport_top && bottom < viewport_bottom) ||
           (top > viewport_top && bottom <= viewport_bottom) ||
           (height > viewport_height && top <= viewport_top && bottom >= viewport_bottom)
}

function getViewPortSize(Name) {
    var size;
    var name = Name.toLowerCase();
    var document = window.document;
    var documentElement = document.documentElement;
    if (window["inner" + Name] === undefined) {
        // IE6 & IE7 don't have window.innerWidth or innerHeight
        size = documentElement["client" + Name];
    }
    else if (window["inner" + Name] != documentElement["client" + Name]) {
        // WebKit doesn't include scrollbars while calculating viewport size so we have to get fancy

        // Insert markup to test if a media query will match document.doumentElement["client" + Name]
        var bodyElement = document.createElement("body");
        bodyElement.id = "vpw-test-b";
        bodyElement.style.cssText = "overflow:scroll";
        var divElement = document.createElement("div");
        divElement.id = "vpw-test-d";
        divElement.style.cssText = "position:absolute;top:-1000px";
        // Getting specific on the CSS selector so it won't get overridden easily
        divElement.innerHTML = "<style>@media(" + name + ":" + documentElement["client" + Name] + "px){body#vpw-test-b div#vpw-test-d{" + name + ":7px!important}}</style>";
        bodyElement.appendChild(divElement);
        documentElement.insertBefore(bodyElement, document.head);

        if (divElement["offset" + Name] == 7) {
            // Media query matches document.documentElement["client" + Name]
            size = documentElement["client" + Name];
        }
        else {
            // Media query didn't match, use window["inner" + Name]
            size = window["inner" + Name];
        }
        // Cleanup
        documentElement.removeChild(bodyElement);
    }
    else {
        // Default to use window["inner" + Name]
        size = window["inner" + Name];
    }
    return size;
}

function getFirstItem() {
    for (var key in $.at.errors)
        return key;
    for (var key in $.at.failures)
        return key;
    for (var key in $.at.ignored)
        return key;
    for (var key in $.at.warnings)
        return key;
    return 0;
}

function getLastItem() {
    var item = 0;
    for (var key in $.at.warnings)
        item = key;
    if (item === 0) {
        for (var key in $.at.ignored)
            item = key;
    }
    if (item === 0) {
        for (var key in $.at.failures)
            item = key;
    }
    if (item === 0) {
        for (var key in $.at.errors)
            item = key;
    }
    return item;
}

function getItemByOffset(offset) {
    item = $.at.selectedItem;
    if (offset > 0) {
        for (var i = 0; i < offset; i++) {
            var newItem = skipOneForward(item);
            if (item === newItem)
                break;
            item = newItem;
        };
    } else {
        for (var i = 0; i < (offset * -1); i++) {
            var newItem = skipOneBackward(item);
            if (item === newItem)
                break;
            item = newItem;
        };
    }
    return item;
}

function getNextItem() {
    return skipOneForward($.at.selectedItem);
}

function skipOneForward(current) {
    var foundItem = false;
    for (var key in $.at.errors) {
        if (foundItem)
            return key;
        if (key === current)
            foundItem = true;
    };

    for (var key in $.at.failures) {
        if (foundItem)
            return key;
        if (key === current)
            foundItem = true;
    };

    for (var key in $.at.ignored) {
        if (foundItem)
            return key;
        if (key === current)
            foundItem = true;
        if (key === current)
            foundItem = true;
    };

    for (var key in $.at.warnings) {
        if (foundItem)
            return key;
        if (key === current)
            foundItem = true;
    };

    return current;
}


function getPreviousItem() {
    return skipOneBackward($.at.selectedItem);
}

function skipOneBackward(current) {
    var previous = current;
    for (var key in $.at.errors) {
        if (key === current)
            return previous;
        previous = key;
    };

    for (var key in $.at.failures) {
        if (key === current)
            return previous;
        previous = key;
    };

    for (var key in $.at.ignored) {
        if (key === current)
            return previous;
        previous = key;
    };

    for (var key in $.at.warnings) {
        if (key === current)
            return previous;
        previous = key;
    };

    return current;
}

function compileTemplate(name, destination, data) {
    var source   = $(name).html();
    var template = Handlebars.compile(source);
    var html    = template(data);
    $(destination).html(html);
}
