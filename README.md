# NTeoTestBuildeR

The repository was created for the talk I gave at the Krakow .NET Developers Group. The meeting took place on 17 April 2024 at 18:00 in the HEVRE club. For more details, check out our [Meetup Event](https://www.meetup.com/pl-PL/kgd-net/events/300191480/).

_Stay tuned for the link to the recording of this presentation on our YouTube channel, coming soon._

The main goal of this repository is to present a test builder that makes it easy to write application tests. The strength of this repository lies in the test project, where a convenient test builder has been meticulously crafted. This builder seamlessly integrates technologies such as ASP.NET Core, Verify, Wiremock, Testcontainer and Test Builder, which together provide significant power and accelerate the writing of tests in a proper manner.

> The To-Do List application provided here is a simple implementation, deliberately kept simple to facilitate understanding of the test engine we use. Its purpose is to facilitate the demonstration of our testing capabilities, rather than being the main focus of this repository. The main strength of this repository lies in its test project, where a powerful and convenient test builder has been carefully crafted, integrating technologies such as ASP.NET Core, Verify, Wiremock, Testcontainer and Test Builder to speed up the writing of tests.

# 🔊 ASP .NET Core + Verify + Wiremock + Testcontainer + Test Builder

Verify provides convenient assertion building based on snapshot testing. Wiremock effectively emulates third party API interfaces. Testcontainers simplifies database management. Test Builder is a sophisticated solution that integrates these tools into a cohesive whole, maximising the efficiency and clarity of the testing process. I provided a comprehensive solution that prevented the application from being cemented with mocks at lower layers. I ensured that the application run for testing purposes was executed in its entirety, exactly as it would be in production, without modifying the IoC container using mocks. This solution allows us to avoid the slowdown caused by having to adapt old test mocks to the new code.