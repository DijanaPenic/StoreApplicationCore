# Store Application 

## External Login

Blog post: https://chsakell.com/2019/07/28/asp-net-core-identity-series-external-provider-authentication-registration-strategy/

Rule #1: Users can sign in only if their email is confirmed.
Rule #2: Users authenticated by an external provider are considered trusted.
Rule #3: Users authenticated by an external provider that select the option to associate the email address used in the provider with an existing account registered with a different email address, will have to confirm the external provider association through the existing account’s email address.
Rule #4: An association can only happen with already confirmed accounts.
Rule #5: Users authenticated by an external provider but have an existing account with the same email address that hasn’t been confirmed, have to confirm the association which eventually will automatically confirm the existing account as well. 

Authentication:

External login already exists - authenticate
External login doesn't exist:
          The same email address is found:
              * email is confirmed - add external login and authenticate
              * email is not confirmed - send token to the email
          The same email address is not found - user can choose to register a new account or to associate external login with the existing account (different email)
              * register a new account - the system will create a new user and authenticate
              * associate with the existing account:
                  * email is confirmed - send token to the email
                  * email is not confirmed - return an error

## Two-Factor Authentication

// Good blog post about two factor authentication: https://chsakell.com/2019/08/18/asp-net-core-identity-series-two-factor-authentication/