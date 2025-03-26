# Invene Take Home Challenge

This is a sample application for redacting PHI from lab orders. It uses [Next.js](https://nextjs.org/) (a production-ready framework built on React) for the frontend and .NET for the backend. Next.js was used because [React's recommendation](https://react.dev/learn/creating-a-react-app) is to start a green-field project with a framework so that common problems like routing can be tackled easily. 

## Running the Project

### Backend

#### Installing Dependencies

The backend uses .NET 6. If you do not have it already installed, please [click here](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) to install it.

#### Running the API

After installing .NET 6

1. Open terminal or command line and navigate to the `/backend` directory in this repo (where you have downloaded it)
2. Run `dotnet dev-certs https --trust` to generate a Developer certificate. Follow any prompts that may be displayed
3. Run `dotnet run --project InveneTakeHomeChallenge.LabOrderProcessor` to build and start the API. Follow any prompts that may be displayed

#### Running the Unit Tests

1. Open terminal or command line and navigate to the `/backend` directory in this repo (where you have downloaded it)
2. Run `dotnet test` to build and run the unit tests

### Frontend

#### Installing Dependencies

The frontend requires Node v20.17.0 or higher. You can install it by following the instructions [here](https://nodejs.org/en/download) if you don't already have it installed. You can upgrade your node version by directly through npm by running `npm install -g n`, or you can use `nvm` to install a copy of the latest version. More instrucitons can be found [here](https://www.freecodecamp.org/news/how-to-update-node-and-npm-to-the-latest-version/).

#### Running the App

1. Open terminal or command line and navigate to the `/frontend` directory in this repo (where you have downloaded it)
2. Run `npm install` to download the project's dependencies
3. Run `npm run dev` to start the app. By default, the app is available on `http://localhost:3000`. See the output in your console for more details

## Using the App

The app provides a simple interface to select a lab order file (limited to `.txt` files only) and submit it for processing. Test lab orders can be found in `/Test Lab Orders`. Once processing is complete, you will recieve a success message. The sanitized file can be accessed under `path/to/this/repo/backend/InveneTakeHomeChallenge.LabOrderProcessor/bin/Deebug/net6.0/` named as `<filename>_sanitized.txt`. .NET's `AppDomain.CurrentDomain.BaseDirectory` is used to save the sanitized file so that the backend can run smoothly across multiple platforms.

## Approach

The following assumptions were made about the input lab order files for simplicity and to respect the time frame of this project:

- Lab orders will only be `.txt` files
- Relevant PHI will be present in a `phiKey: value` format, with one key-value pair per line

The following attributes are considered PHI in this project and are used as `phiKey`s

- patient name
- name
- date of birth
- dob
- social security number
- ssn
- address
- home address
- phone number
- number
- email address
- email
- medical record number

### Processing the Lab Order

The lab order is processed in 2 steps to robustly redact all PHI. For each line in the lab order

1. If the pattern `phiKey: value` exists, then replace `value` with `[REDACTED]`
2. If there are any remaining regex matches for SSN, Date of Birth, Phone Number, Email, and Medical Record Number, then replace those matches with `[REDACTED]`

Step 1 is done first so that `phiKey`s with more free-form patterns, like Name, can be redacted first. If the lab order's structure conforms to the assumptions stated above, then this should redact most of the PHI present in the lab order. Nonetheless, it is possible that some PHI remains due to formatting errors and/or typos. That's why Step 2 is run after Step 1 so that it can redact any remaining regex patterns. This helps to robustly redact PHI from the lab order.

**Note**: the regex patterns used in Step 2 were kept simple to respect the time constraints of the project. 

## Backend Architecture

The backend is architected around Domain-Driven Design (DDD) and Vertical-Slice Architure (VSA).

- `InveneTakeHomeChallenge.LabOrderProcessor` is meant to represent a bounded context in DDD
- By splitting projects around bounded contexts, we create fully self-contained services
- This pattern promotes clear boundaries between services, helping decouple them, test them in isolation, speed up development, and even prepare the project for a microservices architecture if ever desired. 

Each bounded context (`InveneTakeHomeChallenge.LabOrderProcessor` in this project) can organize its files and resources as is most appropriate for it. In this project, we use VSA and create isolated feature slices.

- This helps create even smaller, isolated boundaries between features to promote loose coupling (resulting in less bugs) and simpler testing (changing one feature should not impact other features drastically, if ever)
- It simplifies file organization by 
    - Placing related files close to each other
    - Reducing the need to create more abstractions to decouple services from each other
- It promotes the development of business-centric features and helps align business concerns and verbiage with technical concerns and verbiage
- Can easily be combined with patterns like Mediator and CQRS to develop a high performing system that can be maintained with more ease and can also evolve quickly per business needs

That's why all files related to the `SanitizeLabOrder` feature (Controller, Handler, Repository, and interfaces) live in the same feature folder. Note that VSA may still have shared components, like entitities, and maybe even shared repositories, that live in their own folders. Nonetheless, it is usually suggested to keep features as decoupled from each other as possible to ease testing and reduce bugs. Refer to [this page](https://www.milanjovanovic.tech/blog/vertical-slice-architecture) for mor details about VSA.

## Frontend Architecture

The frontend architecture was kept relatively simple to respect the time constraints of this project. It uses [MUI](https://mui.com/) as its component library and Next.js' app router for routing. 

## Future Enhancements

Given more time, here are some ways the project can be improved in the future.

### Backend

- Add [MediatR](https://github.com/jbogard/MediatR) to implement the Mediator pattern. This can be powerful to implement logging and and exception handling more centrally in the app. It also promotes the CQRS pattern
- Add integration tests to test repositories 

### Frontend

- Add a central store like [React Redux](https://react-redux.js.org/) or [Zustand](https://zustand.docs.pmnd.rs/getting-started/introduction) to better manage `Loading` and `Snackbar` components.
- Add unit test and end-to-end tests in with a testing framework like [Cypress](https://www.cypress.io/)