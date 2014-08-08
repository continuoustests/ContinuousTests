Feature: SMS payments
	In order to instantly make money transfers
	As a mobile bank user
	I want to use SMS to send money to other users

Scenario: Send money between two registered users
	Given following users are registered
	| Phone number | Balance |
	| 92748326     | 100     |
	| 95473893     | 50      |
	And payment fee is as follows
	| Payment type | Payer fee | Collector fee |
	| Private      | 1         | 0             |
	When user sends SMS
	| Phone number | Message         |
	| 92748326     | PAY 10 95473893 |
	Then following SMS should be sent
	| Phone number | Message                                                                                   |
	| 92748326     | You paid 10 to 95473893. Your new balance is 89. Thank you for using InMemory Bank.       |
	| 95473893     | You received 10 from 92748326. Your new balance is 60. Thank you for using InMemory Bank. |

Scenario: Send money from unregistered user
	Given user with phone number 92748326 is not registered
	When user sends SMS
	| Phone number | Message |
	| 92748326 | PAY 10 95473893 |
	Then following SMS should be sent
	| Phone number | Message |
	| 92748326 | In order to use InMemory Bank you need to register. Command is cancelled. |
	And no SMS should be sent to 95473893

Scenario: Send money to unregistered user
	Given user with phone number 92748326 is registered
	But user with phone number 95473893 is not registered
	When user sends SMS
	| Phone number | Message |
	| 92748326 | PAY 10 95473893 |
	Then following SMS should be sent
	| Phone number | Message |
	| 92748326 | You can not send money to unregistered user (95473893). Command is cancelled. |
	And no SMS should be sent to 95473893

Scenario: Insufficient balance
	Given following users are registered
	| Phone number | Balance |
	| 92748326     | 10     |
	| 95473893     | 50      |
	When user sends SMS
	| Phone number | Message         |
	| 92748326     | PAY 20 95473893 |
	Then following SMS should be sent
	| Phone number | Message                                                                                   |
	| 92748326     | Not enough funds to pay 20 to 95473893. Your current balance is 10. Command is cancelled. |
	And no SMS should be sent to 95473893
