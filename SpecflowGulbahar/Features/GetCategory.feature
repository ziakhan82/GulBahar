Feature: GetCategory

A short summary of the feature
As a user
  I want to retrieve a category by its ID
  So that I can view its details


@tag1
Scenario: Valid Category ID
    Given the database contains a category with ID 1
    When I retrieve the category by ID 1
    Then the result should be a CategoryDTO with ID 1

  Scenario: Invalid Category ID
    Given the database does not contain a category with ID 999
    When I retrieve the category by ID 999
    Then the result should be an empty CategoryDTO