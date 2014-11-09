#pragma once

template<typename ValueType>
struct SimpleArray {
    typedef ValueType value_type;

private:
    value_type* _data;

public:
    SimpleArray(size_t count) {
        _data = NULL;
        if (count > 0)
		    _data = new value_type[count]; 
	}

    ~SimpleArray() { 
        if (_data != NULL) delete[] _data; 
    }

    operator value_type* () { return _data; }
    operator const value_type* () const { return _data; }
};
