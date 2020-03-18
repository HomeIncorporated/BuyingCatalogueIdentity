﻿Feature: Display an Organisations by Id in the Authority Section
  As an Authority User
  I want to view an Organisation information for a single orgnisation
  So that I can manage access to the Buying Catalogue

Background:
	Given Organisations exist
		| Name           | OdsCode |
		| Organisation 1 | Ods 1   |
		| Organisation 2 | Ods 2   |

@5147
Scenario: 1. Get the details of a single organisation
	Given an authority user is logged in
	When a GET request is made for an organisation with name Organisation 1
	Then a response with status code 200 is returned
	And the string value of name is Organisation 1
	And the string value of odsCode is Ods 1

@5147
Scenario: 2. Organisation is not found
	Given an authority user is logged in
	And an Organisation with name Organisation 3 does not exist
	When a GET request is made for an organisation with name Organisation 3
	Then a response with status code 404 is returned

@5147
Scenario: 3. If a user is not authorised then they cannot access the organisation
	When a GET request is made for an organisation with name Organisation 1
	Then a response with status code 401 is returned

@5147
Scenario: 4. Service Failure
	Given an authority user is logged in
	And the call to the database to set the field will fail
	When a GET request is made for an organisation with name Organisation 1
	Then a response with status code 500 is returned