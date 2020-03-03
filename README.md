# Custom Policy Provider Demo 

This repo is a part of the ASP.NET Core: Custom Authorization Policies With Multiple Requirements article published at &lt;url>

## Running The Demo Application

These steps follow the steps to run the demo application with Visual Studio 2019 but you can always build and run it with the console in any environment.

1. Clone the repository
2. Open the solution with Visual Studio 2019
3. Run the application with Ctrl+F5 
4. Check the following links:
    - http://localhost:50001/weatherforecast/or - Multiple requirements in a single attribute, all valid.
    - http://localhost:50001/weatherforecast/or/partial - Multiple requirements in a single attribute, some valid.
    - http://localhost:50001/weatherforecast/or/fail - Multiple requirements in a single attribute, all invalid.
    - http://localhost:50001/weatherforecast/and - Multiple requirements in multiple attributes, all valid.
    - http://localhost:50001/weatherforecast/and/fail - Multiple requirements in multiple attributes, some valid or all invalid.
    - http://localhost:50001/weatherforecast/empty - Empty attribute with no requirements.