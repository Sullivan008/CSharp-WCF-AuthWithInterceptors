# C# - WCF - Authentication and Authorization with Interceptors. [Year of Development: 2018]

About the application technologies and operation:

### Technologies:
- Programming Language: C#
- FrontEnd Side: Windows Presentation Foundation (WPF) - .NET Framework 4.6.1
- BackEnd Side: Windows Communication Foundation (WCF) - .NET Framework 4.6.1

### Installation/ Configuration:

1. Restore necessary Packages on the selected project, run the following command in **PM Console**

   ```
   Update-Package -reinstall
   ```
     
### About the application:

In this application, I am going to show how to use Authorization and Authentication using a WCF service in Enterprise Architecting standards. 
 
#### The core aspects we cover here are:
- WCF
- ASP.NET Authentication Service
- Custom Authentication
- HTTP Cookies
- Authorization PrincipalPermission Attribute
- Thread CurrentPrincipal
- Message Interceptors

#### In a quick summary the following are the activities involved: 
- Create a WCF Service Application.
- Add an AuthenticationService.svc reusing ASP.NET Authentication Service.
- Create User Validator class.
- Enable Custom Authentication in Global.asax.
- Return Cookie if valid User.
- Modify service configuration.
- Create a ExecuteOperationsService.svc with three different method named SumOperation, ReadOperation and WriteOperation.
- Decorate Service Methods with PrincipalPermission attribute for Authorized Access only.
- Decorate ExecuteOperationsService class with AspNetCompatibilityRequirements attribute.
- Implement Interceptors in the Server Application.
- Implement the identity setting code to Interceptors in the Service Application.
- Modify the service-side code to include Role instead of Name.
- Use Encrypted Ticket for storing User Name and Roles.
- Create the Client Application and add references to both services.
- Implement Interceptors in the Client Application.
- Create Authentication Service instance and invoke the Login() method.
- Receive the cookie and store it.
- Create a ExecuteOperationsService instance and invoke SumOperation, ReadOperation and WriteOperation.
- Attach the Cookie to the ExecuteOperationsService client

### Useful Highlights:

**1. Add an AuthenticationService.svc reusing ASP.NET Authentication Service.**
 
Add a new WCF Service and name it AuthenticationService.svc. Delete the associated files since we are going to expose the ASP.NET Authentication Service.
- Delete AuthenticationService.cs
- Delete IAuthenticationService.cs

#### Replace the contents of AuthenticationService.svc with the following:

   ```C#
   <%@ ServiceHost Language="C#"
       Service="System.Web.ApplicationServices.AuthenticationService"
       Factory="System.Web.ApplicationServices.ApplicationServicesHostFactory" %>
   ```

**2. Enable Custom Authentication in Global.asax and return a Cookie if Valid User.**
 
Add a new item Web > **Global Application Class** into the project.

#### Replace the Appilcation_Start event as follows:

   ```C#
    protected void Application_Start(object sender, EventArgs e)
    {
        AuthenticationService.Authenticating +=
            AuthenticationService_Authenticating;
    }
   ```

#### Add the following event handler:

   ```C#
    private void AuthenticationService_Authenticating(object sender, AuthenticatingEventArgs e)
    {
        string roles = string.Empty;

        e.Authenticated = UserIsValid(e, ref roles);
        e.AuthenticationIsComplete = true;

        if (e.Authenticated)
        {
            string encryptedValue = FormsAuthentication.Encrypt(CreateFormsAuthenticationTicket(e, roles));

            OperationContext.Current.OutgoingMessageProperties[HttpResponseMessageProperty.Name] = SetSetCookieInResponseChannelHeaders(encryptedValue);
        }
    }
   ```

The method extracts the User Name and Password from the Custom Credential object of the Authentication Event Argument. Then it validates the User Name and Password with our UserValidator class.
 
The property Authenticated represents true / false if a user is valid or not respectively.

**3. Enable Authentication Service and ASP.NET Compatibility**

Now we need to modify the web.config file to include the following:
- Enable Authentication Service
- Enable ASP.NET Compatibility

   ```XML
  <system.web.extensions>
    <scripting>
      <webServices>
        <authenticationService enabled="true"/>
      </webServices>
    </scripting>
  </system.web.extensions>
   ```

**4. Implement Interceptors in the Server Application.**

This is because the above code of attaching a cookie to the Operation Context seems to be a tedious job every time we need to do a service call. We can move these tasks to the background using WCF Interceptors.
 
MSDN: WCF Data Services enables an application to intercept request messages so that you can add custom logic to an operation. You can use this custom logic to validate data in incoming messages. You can also use it to further restrict the scope of a query request, such as to insert a custom authorization policy on a per request basis.

The following are the activities involved in this step.
- Add Interceptor Behavior inside Server web.config.
- Create the Interceptor Behavior class. (See in the project)
- Create the **IdentityMessageInspector** class. (See in the project)

#### The following code to the web.config file of the service just under <system.serviceModel>

   ```XML
  <system.serviceModel>
    <services>
      <service name="AuthenticationAndAuthorization.ExecuteOperationsService" behaviorConfiguration="InterceptorBehavior" />
    </services>

    <behaviors>
      <serviceBehaviors>
        <behavior>
          <serviceMetadata httpGetEnabled="true" />
          <serviceDebug includeExceptionDetailInFaults="true" />
        </behavior>
        <behavior name="InterceptorBehavior">
          <serviceDebug includeExceptionDetailInFaults="true" />
          <interceptorBehaviorExtension />
        </behavior>
      </serviceBehaviors>
    </behaviors>

    <extensions>
      <behaviorExtensions>
        <add name="interceptorBehaviorExtension" type="AuthenticationAndAuthorization.Extensions.IDispatchMessageInspector.InterceptorBehaviorExtension, AuthenticationAndAuthorization, Version=1.0.0.0, Culture=neutral"/>
      </behaviorExtensions>
    </extensions>

    <serviceHostingEnvironment aspNetCompatibilityEnabled="true" multipleSiteBindingsEnabled="true" />
  </system.serviceModel>
   ```

**5. Cookie Encryption.**

Our cookie contains information that is easily readable by HTTP Examining Utilities like Fiddler. This makes the cookie information prone to security threats.

**FormsAuthenticationTicket**: The System.Web.Security namespace provides a convenient class for us. We can store the user name and roles information inside this class instance. The ticket class also provides expiry and encryption facilities.

#### The following code creates the ticket:

   ```C#
    private FormsAuthenticationTicket CreateFormsAuthenticationTicket(AuthenticatingEventArgs e, string roles) =>
        new FormsAuthenticationTicket(1,
            e.UserName,
            DateTime.Now,
            DateTime.Now.AddHours(24),
            true,
            roles,
            FormsAuthentication.FormsCookiePath);
   ```

#### The following code encrypts and decrypt the ticket:

   ```C#
    string encryptedValue = FormsAuthentication.Encrypt(CreateFormsAuthenticationTicket(e, roles));

    private FormsAuthenticationTicket GetDecryptTicket(string encryptedTicket) =>
        FormsAuthentication.Decrypt(encryptedTicket);
   ```

#### Now we can pass the ticket as name value pair through the message properties:

   ```C#
    private HttpResponseMessageProperty SetSetCookieInResponseChannelHeaders(string cookieValue)
    {
        HttpResponseMessageProperty response = new HttpResponseMessageProperty();
        response.Headers[HttpResponseHeader.SetCookie] = FormsAuthentication.FormsCookieName + "=" + cookieValue;

        return response;
    }
   ```

#### You need to specify the Cookie Name, Machine Key Properties inside the web.config as shown below:

   ```XML
    <authentication mode="Forms">
      <forms slidingExpiration="true" name="AuthCookie" protection="All" timeout="20"/>
    </authentication>

    <machineKey decryption="AES" validation="SHA1"
                decryptionKey="1523F567EE75F7FB5AC0AC4D79E1D9F25430E3E2F1BCDD3370BCFC4EFC97A541"
                validationKey="33CBA563F26041EE5B5FE9581076C40618DCC1218F5F447634EDE8624508A129"/>
   ```

**6. Implement Interceptors in the Client Application.**

The following are the activities involved in this step.
- Add Interceptor Behavior inside Client App.config
- Create the Interceptor Behavior class. (See in the project)
- Create the **CookieMessageInspector** class. (See in the project)

#### The following code to the App.config file of the service just under <system.serviceModel>

   ```XML
  <behaviors>
    <endpointBehaviors>
      <behavior name ="InterceptorBehavior">
        <interceptorBehaviorExtension />
          </behavior>
    </endpointBehaviors>
  </behaviors>
   ```

#### Change the Behavior Configuration element of the ExecuteOperations Service section as shown below.

   ```XML
  <endpoint address="http://localhost:8046/ExecuteOperationsService.svc" binding="basicHttpBinding" bindingConfiguration="BasicHttpBinding_IExecuteOperationsService"
    contract="ExecuteOperationsServiceReference.IExecuteOperationsService" name="BasicHttpBinding_IExecuteOperationsService" behaviorConfiguration="InterceptorBehavior"/>
   ```
