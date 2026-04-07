# 03. Architectural Styles and Patterns

##### 1. What are the cons and pros of the Monolith architectural style?

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

##### 2. What are the cons and pros of the Microservices architectural style? 

The main idea of the style is splitting into a set of smaller services (monolithic applications) with ability of interactions between each other if needed. So  
➕ High code decoupling achieved, because there are clear responsibilities and bounds between services, what makes them better to understand, review & maintain.  
➕ Code ownership may be applied between development teams - allows parallel work without blocking each other.  
➕ Every service follows its contract (API), so the tech stacks of the service may be different (language, DB type, and so on) but always following the contract. There is no technical dependency (library version and so on) between services, so migration can be done easily within the service.  
➕ Deployment of a single service don't require redeployment of other ones.  
➕ Individual scaling of a single service is possible.  
➕ Fault isolation. If one service crashes or has technical issues (memory leaks) that other services may not face with.  
➕ Polyglot persistence - the service can rely on the type of DB optimal for the service needs (structured/unstructured data and so on).  

But with the number of services growing several problems appear:  
➖ Distributed system complexity: inter-process communication mechanism required, latency, failures, retries and so on become current problems to face with.  
➖ Data consistency is another point to think about. Especially if no transactions applied across services.  
➖ Difficult debugging. The tracing of the request processed across multiple services is a difficult task.  
➖ DevOps efforts to support monitoring, logging, health checks in every service.  
➖ Testing complexity. The inter-service dependency may lead to difficulty testing scenarios and additional preparations (e.g. for e2e or integration tests).  
➖ A single API Gateway is recommended.  
➖ It takes some time to extract services from monolith and split the responsibilities. May be much harder if monolith was not properly organized.  

##### 3. What is the difference between SOA and Microservices?

Both rely on the services as the main component but have differences in several aspects  

| Aspect             | SOA (Service-Oriented Architecture)                                                                                                   | Microservices                                                                                                                                   |
| ------------------ | ------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------- |
| **Sharing style**  | "Share‑as‑much‑as‑possible". Services are designed to be reused across many consumers, often via a shared enterprise bus or registry. | "Share‑as‑little‑as‑possible". Services are small, autonomous, and avoid tight coupling; duplication is accepted to reduce shared dependencies. |
| **Service size**   | Large, enterprise-wide services (often ESB-mediated)                                                                                  | Small, single responsibility (bounded contexts)<br>                                                                                             |
| **Communication**  | Heavyweight protocols (SOAP, WSDL, XML) over Event Service Bus                                                                        | Lightweight through API (REST/JSON, gRPC, async messaging)                                                                                      |
| **Code ownership** | Centralized                                                                                                                           | Teams own their services                                                                                                                        |
| **Technology**     | Often same tech stack, vendor products (Oracle, IBM, etc)                                                                             | Polyglot                                                                                                                                        |
| **Deployment**     | Infrequent, centralized                                                                                                               | Independent, continuous deployment                                                                                                              |
| **Data storage**   | Shared database                                                                                                                       | Database per service, decentralized data                                                                                                        |
| **Philosophy**     | Top-down, enterprise-wide initiative                                                                                                  | Bottom-up, product-orien                                                                                                                        |

##### 4. What does hybrid architectural style mean? Think of your current and previous projects and try to describe which architectural styles they most likely followed.

The name says it all - a hybrid architectural style means a style for a system that combines two or more architecture styles (for example, monolith + microservices, or event‑driven + serverless) into a single cohesive design, taking the strengths of each while managing the trade‑offs. So it's not one "pure" style, but a mix with clearly visible signs (advantages) of several styles.

In my current project codebase there is a solution containing several Function Apps (with Azure Functions inside), that use services to manipulate the data in our system and interact with other systems within and outside the corporative network.
This set of Function Apps considered to be supportive to the main low-code MS Dynamics-based application. These apps use services that make requests to Dataverse (Dynamics app DB) or able to do HTTP requests to APIs.

The solution items are divided into a couple of projects: a library with Models (Avro models and DTOs), Dynamics models (and their services), Mapping services, Persistence project containing database services (to work with Postgre and SQL) - that says we follow a clean architecture style with Core, Infrastructure and Presentation (in my case - Entry Points) presented by Function Apps.

The functions are used for integration with other systems using Kafka (using Avro models) or API requests triggered by a specific event in the Dynamics-based app (creation, updates of entities).

These signs tell me our solution is a hybrid of Serverless + Event‑driven + Clean Architecture.

##### 5. Name several examples of the distributed architectures. What do ACID and BASE terms mean.

Client-server, Layered Distributed Architecture (with different layers on different machines: UI, API/BL, Data access layer), Event-driven architecture.

Both ACID and BASE describe transaction guarantees in databases.  

ACID is a set of rules for transactions in SQL to guarantee safety, reliability and correctness in a database.

- Atomicity. Every transaction is “all or nothing" - if any part fails, the whole transaction is rolled back.
- Consistency. The database stays in a valid state before and after the transaction (constraints, rules, invariants are preserved).
- Isolation. Concurrent transactions don’t interfere with each other, they look like they run one after another.
- Durability. Once a transaction is committed, its changes survive failures (e.g., stored on disk or replicated).

ACID systems prioritize **correctness and consistency** over availability and speed – great for financial systems, orders, etc., but can be harder to scale horizontally.

BASE is a more relaxed, availability‑oriented model used by many NoSQL databases:

- Basically Available. The system stays available even under partial failures, it may accept requests but return stale or incomplete data.
- Soft state. The system’s state can change over time without external input (e.g., because of replication lag between nodes).
- Eventually Consistent. After all updates stop, the system will eventually converge to a consistent state; it does not guarantee immediate consistency.

BASE systems prioritize **availability, scalability, and performance** over strict consistency – good for user feeds, logs, analytics, and large‑scale distributed services.

##### 6. Name several use cases where Serverless architecture would be beneficial.

Serverless is a good style for the applications no need to worry about scaling or long-running infrastructure. Typical usage cases are event-driven architectures (Kafka, Service Bus, etc), batch and background jobs with scheduled runs and POCs implementation - works like "pay as you go".