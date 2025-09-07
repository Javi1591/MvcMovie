# MvcMovie App
## Xavier Nazario

# Description
This is a web application that shows a list of movies including the title, genre, release date, price, and parental guidance rating. It features include allowing a user to creating, editing, and deleting entries into the list. Users are also able to search for movies by filtering through genre. 

## How to run the application locally
- Step 1: Clone the repository by navitgating to http://github.com/Javi1591/MvcMovie
- Step 2: Navigate to the project directory, using command "cd MvcMovie" in the command console
- Step 3: Install the needed dependencies, as noted below, using the command "npm install"
 - Needed dependencies for the MvcMovie application to successfully run include:
   - Packages -
     - Microsoft.EntityFrameworkCore.SqlServer (9.0.0)
     - Microsoft.EntityFrameworkCore.Tools (9.0.0)
     - Microsoft.VisualStudio.Web.CodeGeneration.Design (9.0.0)
   - Frameworks -
     - Microsoft.AspNetCore.App
     - Microsoft.NETCore.App
- Step 4: Run the application using the command "npm start"

## Architectual Principles and Dependency Injection
This program has been structured by keeping the business and user-interface code separate. By following the 
separation of concern principle, the business and user-interface logic allows for the program to grow and change
while being able to efficiently maintain and test the code over time. This was achived through the use of dependency
injection in order to manage the dependencies in any class. The dependency injection choices include using
constructor injection for needed services, using interfaces, and correct lifetimes like scoped for data services and singleton for shared resources.

## Route Mapping
The route mapping for the `MoviesController` is in the table below:
| Action   | HTTP      | Route                                         | Example URL                  |
|----------|-----------|-----------------------------------------------|------------------------------|
| Index    | GET       | `/movies`                                     | `/movies`                    |
| Details  | GET       | `/movies/details/{id:int}`                    | `/movies/details/5`          |
| Create   | GET       | `/movies/create`                              | `/movies/create`             |
| Create   | POST      | `/movies/create`                              | `/movies/create`             |
| Edit     | GET       | `/movies/edit/{id:int}`                       | `/movies/edit/3`             |
| Edit     | POST      | `/movies/edit/{id:int}`                       | `/movies/edit/3`             |
| Delete   | GET       | `/movies/delete/{id:int}`                     | `/movies/delete/3`           |
| Delete   | POST      | `/movies/delete/{id:int}`                     | `/movies/delete/3`           |
| ByGenre  | GET       | `/movies/bygenre/{genre}`                     | `/movies/bygenre/Comedy`     |
| Released | GET       | `/movies/released/{year:int:min(1900)}/{month:int:range(1,12)?}` | `/movies/released/2010`<br>`/movies/released/2010/5` |
