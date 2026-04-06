# 03. Architectural Styles and Patterns

1. **What are the cons and pros of the Monolith architectural style?**

The main advantage is having everything in one place
➕ Understanding is simple during onboarding.
➕ Development is simple because the code is located in one place with no dependencies.
➕ Debugging is simple, the flow of specific request is easy tracked within the application.
➕ Testing is simple - e2e testing doesn't require any additional preparation steps (just run the application and it's ready to test)
➕ Deployment of a single monolithic unit is also simple, horizontal scaling (multiple instances with load balancer) is supported.
➕ Good for early stages of application development - when the requirements are not finalized and its not clear what architecture to choose in the future.

At the same time the "all in one" approach has its disadvantages, especially with codebase growing:
➖ High code coupling. A big application has tons of code to review & maintain - may lead to spaghetti with difficulties in understanding.
➖ Its' difficult to split the solution between development teams - code ownership approach cannot be used.
➖ Low flexibility of technologies. Usage of same version of libraries is usually required for all the monolith items, so the more code (components) is inside - the more time it will take to migrate.
➖ Application start takes some time, as well as packing and deployment.
➖ Testing is harder because any change (little or big) leads to full regression for the whole monolith. 
➖ Any change leads to re-deploy of the whole monolith. If the build/deployment pipeline fails for some reason, another run of pipeline will take extra time.
➖ Performance issues. When the application is scaled horizontally, but the database will probably remain one and same for all the services. Of course query optimization and replication may be done, but these ways of optimization are pretty limited.

2. What are the cons and pros of the Microservices architectural style?

The main idea of the style is splitting into a set of smaller services (monolithic applications) with ability of interactions between each other if needed.

3. What is the difference between SOA and Microservices?

4. [Open question] What does hybrid architectural style mean? Think of your current and previous projects and try to describe which architectural styles they most likely followed.

5. Name several examples of the distributed architectures. What do ACID and BASE terms mean.

6. Name several use cases where Serverless architecture would be beneficial.