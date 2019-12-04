# Testing overview

Example from Radix

---

Everyone is responsible for quality and testing  
Automate as much as possible  

## Test scope

### In scope


### Out of scope

## Tools

## Test Users


## Levels of testing

### Unit and Integration Testing

Why: fast feedback that functionality, which we are able to test, is not broken

Who: developers

What: unit testing lower level functions and integration testing scenarios relevant for component

When: new tests should be added for any new functionality

Where: Runs on builds on our components (part of docker build)

Relevant for: operator, api, web-console

Current Status: In place for operator and api server where it has been possible

### Contract testing

Why: Will test that the API server doesn't break expected contract from the consumer of the API

Who: developers

What: Relevant for web console
  
When: Should run on changes to the API, web console

Current status: Manually triggered on changes to the web console

### API/regression testing

Why: Testing that the core functionality of the system is not broken

What: Testing functions (happy paths??) on top of Radix API + Webhook, impersonating a Platform User, to emulate the user scenarios within a cluster, with metrics to dashboard. Should run a series of happy paths, . I.e. registering an application, making/emulating a push, checking that build succeeds, and that environment alias is available and can be reached. Should run some identified edge cases? I.e. deleting orphaned environment, building with validation errors

When: Continously??

Where: All clusters??

Current status: Not started [see spike](https://statoil.atlassian.net/browse/OR-1301)

### Functional testing

Why: Testing that new functionality works as according to expectation

When: Code has passed unit and integration testing, as well as code review, and been merged to master

Where: Dev cluster

How: ??

Current status: manual

### Health testing

Why: Tests the health of the cluster. Gives metrics to dashboard

What: Testing Canary APP endpoints

Where: All clusters

Current status: Runs in cluster

### Penetration testing

Why: Verifification of security vulnerabilities

Where: ???

Current status: Runs in cluster as radix-nsp-test Radix app (should it be incorporated into Canary app??), testing that the setup for network policy in the cluster is working. Gives metrics to dashboard

### Security/authorization testing

Why: Test that we don't open up access, that they should not have, to users on the platform

Where: All clusters??

How: Suggested to create a service accounts to impersonate users (Can we test for users that should not have access to platform?). If we test on our Canary app, we don't need to impersonate more than one user. One example could be to test continuously, in the same way as the radix-nsp-test, that a service account with similar access as the Platform User cannot delete/update another Radix application etc. I.e. the Canary app

Current status: not in place

### Load testing

Why: Test of our entire platform or platform components 

What: Should run load testing on API endpoints. See [story](https://statoil.atlassian.net/browse/OR-624)

Where: Development cluster??

Current status: Runs in all clusters, load testing on Canary endpoint using K6

### Resiliency Testing

Why: Testing the ability of the platform to handle unexpected failures

Out of scope for now

### UI Testing

Why: Testing from a user perspective end to end

Out of scope for now, as it is normally considerable cost to maintain. Will be looked at later