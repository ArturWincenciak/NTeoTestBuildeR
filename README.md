# NTeoTestBuildeR

The repository was created for the talk I gave at the Krakow .NET Developers Group. The meeting took place on 17 April 2024 at 18:00 in the HEVRE club. For more details, check out our [KGD .NET Meetup Event](https://www.meetup.com/pl-PL/kgd-net/events/300191480/).

_Stay tuned for the link to the recording of this presentation on our YouTube channel, coming soon._

The main goal of this repository is to present a test builder that makes it easy to write application tests. The strength of this repository lies in the test project, where a convenient test builder has been meticulously crafted. This builder seamlessly integrates technologies such as ASP.NET Core, Verify, Wiremock, Testcontainer and Test Builder, which together provide significant power and accelerate the writing of tests in a proper manner.

> The To-Do List application provided here is a simple implementation, deliberately kept simple to facilitate understanding of the test engine we use. Its purpose is to facilitate the demonstration of our testing capabilities, rather than being the main focus of this repository. The main strength of this repository lies in its test project, where a powerful and convenient test builder has been carefully crafted, integrating technologies such as ASP.NET Core, Verify, Wiremock, Testcontainer and Test Builder to speed up the writing of tests.

# 🔊 ASP .NET Core + Verify + Wiremock + Testcontainer + Test Builder

Verify provides convenient assertion building based on snapshot testing. Wiremock effectively emulates third party API interfaces. Testcontainers simplifies database management. Test Builder is a sophisticated solution that integrates these tools into a cohesive whole, maximising the efficiency and clarity of the testing process. I provided a comprehensive solution that prevented the application from being cemented with mocks at lower layers. I ensured that the application run for testing purposes was executed in its entirety, exactly as it would be in production, without modifying the IoC container using mocks. This solution allows us to avoid the slowdown caused by having to adapt old test mocks to the new code.

## Commits Step by Step

The code in the repository is structured to be able to navigate through commits one by one, progressing further and addressing new challenges while adding new functionalities to the test builder. Let's start by checking out all the commits step by step and comparing the differences in the code.

---

Presentation of the first very simple TODO list application, still without payload validation.

> $ git checkout 601c75f20396aa391d87e6a51eb7f5f31500f9e7 Simple TODO-list App Intro, No Payload Validation Yet

---
The initial version of TestBuilder, which already includes a list of actions but operates on raw HttpResponseMessage objects.

> $ git checkout 183f9047d03cbc4c503b1cbe92c7cac9c502ebb0 Initial version of TestBuilder, working with basic HttpResponseMessage

---
A quick introduction to Verify (still separate from the TODO-list application) and how to set up the Verify.

> $ git checkout fa333f63cf7b00218036ca3c0af790ee571906cf Quick introduction to Verify

---
First use of Verify to test the app, but still a bit rough without deserializing the HttpResponseMessage. Either we don't do deserialization at all, or we deserialize the payload in the test and put it into Verify. The implementation is simple, too simple, lacking all the information.

> $ git checkout c166e9ee08763eb6ef50c4ca345ea9c1f440391b First use of Verify for app testing, without HttpResponseMessage deserialisation yet

---
We add the deserialisation of HttpResponseMessage at the Test Builder level, defining a helper object in which we enclose all the input and output arguments. We then pass this helper object to Verify.

> $ git checkout baa9d55fde2e3263e2f5f0e964bcb8c6b1c576ae Define input and output arguments in a object for Verify

---
I'm adding more happy path tests - laying the groundwork for the next scenario when I want to introduce a state object.

> $ git checkout e5a7460aedf0aa95cf834c5a543ee69bddfb652d Change Title, Tags and Done To-Do Item Tests

---
A complicated test that requires having the concept of a smarter state object.

> $ git checkout e19b31dbfb3dfe4746473770e35b4b9a9e5f9f39 Complex test requiring smarter state object concept

---
We're adding validation and including validation tests (naive implementation with repetitions at first, followed by a clever one without repetitions using extensions).

> $ git checkout 8f6d54e4c4072187ff408e0e334664aaa16ae612 Validation and included validation tests (naive implementation with repetitions)

> $ git checkout 073048b88dfaa03548f7c8dd6b6d6f9d163996b8 Smart extensions for mapping success or error for Verify

---
Switching to a real database and adapting Testcontainers (EF, Postgres, Docker).

> $ git checkout 65408b12b459ca01a26bcd770b41dcdefcb1e51e Setting up real DB (EF, Postgres, Docker)

> $ git checkout 071132dd4f9726df076993db1d54c8b901d6cff7 Testcontainer with DB for testing

---
Singleton for the test application builder factory so that the application starts up exactly once for all tests.

> $ git checkout f2b0fed9b7459aaae91de92a408c353094b55af1 Single instance of app for all tests (singleton)

---
Sort and manipulate results to always get the same result in the snapshot Verify for assertions.

> $ git checkout 008fb2a9d37bcf15b464659953745fcc65eb9d55 Test for getting to-do's by tags in query string (without sorting)

> $ git checkout f80dc9045791eff0ca64d053934cb2ec5925744c Sort/manipulate the actual result before passing it to Verify

---
Presentation of the second external Calendar application with which we will integrate and which we will mock using Wiremock.

> $ git checkout d417ccfab69944738674b1d4b45d804fcda82c0b 3rd-party Calendar app

---
Integration with Calendar without Wiremock (no new tests, old tests are passing). Here, all tests that were not testing new functionality, so everything is still green.

> $ git checkout bcd6c15b025e546413b93f29d22f226a56bf05cb Integration with 3rd-party Calendar app

---
Setting up Wiremock with the first test, here you can also see Wiremock Inspection in action.

> $ git checkout 7a353d5050a5f7748e81fc987f79fbeca0836b24 Test for retrieving items from the calendar (without Wiremock)

> $ git checkout 7c943158e7480e68ff148a9593e127a5f9b2a3e3 Setup Wiremock and run first test with mock of HTTP

---
I'm adding OpenTelemetry for Wiremock to differentiate which mock belongs to which test scenario. The whole solution works in the background. The programmer writing the tests doesn't need to know that OpenTelemetry exists.

> $ git checkout af4581aa3570ef36d444a2a3172d581acb217e6f Second test with second mock leading to ambiguity in matching in Wiremock

> $ git checkout 5e76d47670f769fc7a499a06737b4438d22b9c7d Add OpenTelemetry for clear identification of mocks in Wiremock

---
Counting the invocations of Wiremock mocks to avoid leaving unused mocks that confuse the understanding of what the test does. We don't want to have mocks that are not involved in the test. We want the test to self-check that it uses all mocks.

> $ git checkout a0d0e15108615a532b3c7557f6725f9d53f7073b More tests with external calendar

> $ git checkout d5cb22203bb52d9ea6738427660843c80041fa19 Test self-verifies if a given Wiremock was used the expected number of times

---
We're adding a cache to the application. Normally, we would have to provide a mock for the cache. However, in this solution, we use the cache without mocking it, just as it is in a real production application.

> $ git checkout 3537363af8b47bc107311c94687914621eb9104e Cache feature and its handling, including tests

---
We're adding a new Stats module - presenting its functionality. This application will serve as a simulation of the second module of our application, and then we'll want to test it in our tests.

> $ git checkout 45be71072df8ac75a211de90b23cafe0db5bce85 Stats module

---
We're adding a second Test Builder for the newly created second Stats Module.

> $ git checkout 2c725f7f1a60e6f84ec277bb19d3453305ef39db Tests for Stats module

---
Integration with the Stats module. Here the tests will fail because our application calls itself via an HTTP API. These are two separate modules, but in a monolithic solution, so it's a single deployment artefact, meaning that communication involves calling our own API. However, a difficulty arises here because our HTTP API is virtual and a simulation in the server's memory, so it doesn't have a reachable address that we could access. Therefore, we need to use a trick that will be presented in the next step.

> 52d62f7802a07538cd31226a2029a8557a949d25 Integration with Stats module

---
Provide a solution for calling itself without mocks in Wiremock. This addresses the problem outlined in the previous commit. Thanks to such a lazy function, we can get access to a virtual HTTP client that we can use inside the application (i.e. outside the tests - which is the opposite of what we've been doing, as we've been using the HTTP client outside the application inside the tests).

> $ git checkout 2f27e38b5b8c39d1aa8d5c9d5e68ea6fb8f810c3 Register virtual HTTP client to self
