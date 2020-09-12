Feature: Login
	Test related to logins

Scenario: Login Go path
	Given I enter jerry as username
	And I enter test123 as password
	When I press login
	Then I should get login success with token qwexxerqer

Scenario: Login Invalid Password
	Given I enter jerry as username
	And I enter test1231 as password
	When I press login
	Then I should get login failed with message invalid password