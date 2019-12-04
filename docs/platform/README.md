# The SEPES platform

## Core technologies

- Azure virtual network
- Azure Storage account
- Azure AD
- C# asp.net core
- react.js

## Components

- Sandbox (in azure)
- Azure portal
- Frontend
- Backend
- Database

TODO
- Add description for each component (why, what, how)   
- Add a infrastructur diagram for how all these components connect to each other

## User-flow, authentication and authorization

TODO
- Describe who users interact with different parts of sepes
- Add diagram

## Dataflow

TODO
- Describe how data flows inside sepes and how that is controlled.  
  This will be the most critical for sepes
- Add diagram

## How to build & run

(example)

`docker-compose up --build`

## How to deploy

(example)

1. Merge branch `master` into branch `release` in local repo
   1. Fix any merge problems
1. Push to remote repo
1. Done!

```sh
# Be sure to have latest updates in master
git checkout master
git pull
# Be sure to have latest updates in release
git checkout release
git pull
# Merge and push
git merge master
git push
```

## How to test

[Testing](./testing/)


