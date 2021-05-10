# Andrew's Notes:

## Process and Challenges:

Once I opened up the zip file my first instinct was to open up the readme of course to see what the challenge was, but I made sure to take notes of exactly what the task was asking before diving in. Reading and re-reading before even starting to code.

Next I opened up Postman to test the existing API endpoints to see what data I was working with. Even though the data is seeded locally, I like to see it in action.

It was at this point that I found out that direct reports for certain employees was coming back null. I have not used Entity Framework before, but I did some research and stepped through with the debugger and this is where I found the problem. When I hovered over the employees object and expanded the values in the watcher that's when the values were showing up, but when I was not expanding the employee fields in the debugger the values came back null in Postman. Trial and error it turned out it just needed the Load method called on the Employees object because EF will sometimes just lazy load elements.

Debugging this gave me a better idea of how the service layers were interacting with each other so I could get started on the first part of the task. Since we're just spitting out an Employee object and the number of people reporting to that employee the nice thing is that we already had an object with the full hierarchy, I just needed to figure out how to find the number of people reporting to that person. Ultimately, since every sub employee reports to that higher up no matter how far down the hierarchy they are all we needed to do was flatten out the tree and that would give us the final number.

Finally, compensation, this was the one that caused the most trouble, but mostly because I was overthinking it. Getting compensation was easy, it was the adding portion that was the difficult part. I opted to upsert data that way I could handle the employee only getting one salary value and one endpoint for updating the salary at a whim. That wasn't the hard part, the hard part was that I was receiving an error related to the Compensation object requiring a primary key. I wanted to try keeping the data structure to match the one provided in the documentation, but ultimately ended up adding an autoincremeneting ID to compensation to get rid of the error, but I wanted to try sticking to the plan as much as possible so I started googling for ways to handle it without inserting a primary key, maybe just having a foreign key, but ultimately couldn't get it to work that way. Being more familiar with SQL Server versus this format of seeded data it was a little different to wrap my brain around, but I feel like in the end I got there.

Ran more tests in Postman, and then pushed it out to github. Again apologies for the delay. Looking forward to hearing from the team!


------------------------------------------------------------------------


# Mindex Coding Challenge
## What's Provided
A simple [.NetCore 2.1](https://dotnet.microsoft.com/download/dotnet-core/2.1) web application has been created and bootstrapped 
with data. The application contains information about all employees at a company. On application start-up, an in-memory 
database is bootstrapped with a serialized snapshot of the database. While the application runs, the data may be
accessed and mutated in the database without impacting the snapshot.

### How to Run
You can run this by executing `dotnet run` on the command line or in [Visual Studio Community Edition](https://www.visualstudio.com/downloads/).


### How to Use
The following endpoints are available to use:
```
* CREATE
    * HTTP Method: POST 
    * URL: localhost:8080/api/employee
    * PAYLOAD: Employee
    * RESPONSE: Employee
* READ
    * HTTP Method: GET 
    * URL: localhost:8080/api/employee/{id}
    * RESPONSE: Employee
* UPDATE
    * HTTP Method: PUT 
    * URL: localhost:8080/api/employee/{id}
    * PAYLOAD: Employee
    * RESPONSE: Employee
```
The Employee has a JSON schema of:
```json
{
  "type":"Employee",
  "properties": {
    "employeeId": {
      "type": "string"
    },
    "firstName": {
      "type": "string"
    },
    "lastName": {
          "type": "string"
    },
    "position": {
          "type": "string"
    },
    "department": {
          "type": "string"
    },
    "directReports": {
      "type": "array",
      "items" : "string"
    }
  }
}
```
For all endpoints that require an "id" in the URL, this is the "employeeId" field.

## What to Implement
Clone or download the repository, do not fork it.

### Task 1
Create a new type, ReportingStructure, that has two properties: employee and numberOfReports.

For the field "numberOfReports", this should equal the total number of reports under a given employee. The number of 
reports is determined to be the number of directReports for an employee and all of their direct reports. For example, 
given the following employee structure:
```
                    John Lennon
                /               \
         Paul McCartney         Ringo Starr
                               /        \
                          Pete Best     George Harrison
```
The numberOfReports for employee John Lennon (employeeId: 16a596ae-edd3-4847-99fe-c4518e82c86f) would be equal to 4. 

This new type should have a new REST endpoint created for it. This new endpoint should accept an employeeId and return 
the fully filled out ReportingStructure for the specified employeeId. The values should be computed on the fly and will 
not be persisted.

### Task 2
Create a new type, Compensation. A Compensation has the following fields: employee, salary, and effectiveDate. Create 
two new Compensation REST endpoints. One to create and one to read by employeeId. These should persist and query the 
Compensation from the persistence layer.

## Delivery
Please upload your results to a publicly accessible Git repo. Free ones are provided by Github and Bitbucket.
