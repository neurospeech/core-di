# core-di
Core dependency injection for .NET

## Why CoreDI?

Compact, small and lightweight, so there are others. CoreDI does not recommend constructor injection pattern and implements best of many frameworks.

1. Support for type registration with Attributes
2. Clean and verbose error messages
3. Fully customizable scopes, you can create nested scopes

## Installation

        PM> Install-Package NeuroSpeech.CoreDI

## Register Type with Attributes

        [DIGlobal]
        public class EmailService{
        }

        [DIAlwaysNew]
        public class StringHashProvider{
        }

        [DIScoped]
        public class AppDbContext: DbContext{
        }

        // Register with interface
        [DIScoped(typeof(IRepository))]
        public class AppRepository: GenericRepository: IRepository{
        }


        // Following method registers all attributed types in given assembly
        // As app domain contains many assemlies, registering all types in all assemblies
        // is slow and degrades performance of startup, so assembly registration has to be
        // called explicitly...
        DI.Register(typeof(App).Assembly);




## Register Type with code

        DI.RegisterScoped<AppDbContext>();

        DI.RegisterGlobal<EmailService>();

        DI.RegisterAlwaysNew<StringHashProvider>();

## Global override

        DI.GlobalOverride<EmailService>( ()=> new MockEmailService() );

## Usage

        var emailService = DI.Get<EmailService>();

        var hashProvider = DI.Get<StringHashProvider>();

## DIScope

        DI.Get<AppDbContext>(); // error no scope...

        var scope = DI.NewScope();

        var db = DI.Get<AppDbContext>(scope);


        var childScope = DI.NewScope(scope);

        var childDb = DI.Get<AppDbContext>(childScope);

        Assert.NotEqual(db,childDb);


# License

    MIT
