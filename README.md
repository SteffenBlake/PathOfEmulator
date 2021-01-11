# PathOfEmulator
A bare bones PathOfExile.com API emulator

See https://www.pathofexile.com/developer/docs/api-resources for documentation, though not all API endpoints are currently supported.

# Download

Get the latest release here: https://github.com/SteffenBlake/PathOfEmulator/releases/latest

# API

Currently supported endpoints:

## OAuth

`/oauth/authorize` OAuth 2.0 endpoint

`oauth/token` OAuth 2.0 Access Token endpoint (pending, need confirmation on if this is the correct endpoint)

# Profile (requires 'profile' scope and OAuth Access Token)

`/api/profile` Profile Endpoint, via Access Token, requires "profile" scope

# Characters (Does not require Access Token)

`/character-window/get-characters` Character list endpoint by Account name

# How to use

Step 1 (optional): Configure the following values in AppSettings.json:
* "ActiveUser" - Specifies which user from the "Data" table will be attempting the login process during pseuo OAuth. Can just leave it as Steve if you want.
* "Client.Id" - Can be anything, just ensure you use the correct value during both of the OAuth steps. Case sensitive. Might be a good idea to set this as the same Client Id you would want to use in production
* "Client.Secret" - Same as above, ensure you use the correct value during the Access token step. Case sensitive. **Definitely dont set this to be the same as your production secret, probably best to just leave it as "Test"**
* "Oauth.AccessTokenLifetimeSeconds" - Set if you want Access Token Expiry functionality. I need to confirm with GGG if they have expirations on their access tokens they give out, if not I will just completely remove this functionality. For now, can just be left at 0 (which causes tokens to never expire)
* "Urls" - Semicolon delimited list of Urls to listen on. You can specify your preferred port to listen on here.

Step 2: Execute the application on windows (Might need to run it as admin), or use `dotnet run` on linux. Terminal will inform you what url the application is now listening on.

Step 3: Open up said url in your browser. Should automatically redirect you to the Swagger interface.
* Swagger Documentation: https://swagger.io/tools/swagger-ui/

Run the program, it will inform you it is listening. Explore the swagger UI, see what endpoints are currently supported, try it out!

# Found a bug?

Report it here: https://github.com/SteffenBlake/PathOfEmulator/issues

# To-Do

- [ ] Add config for "User doesnt have characters public"
- [ ] Make the "Fails to login" config for users actually behave correct
- [ ] Double check with GGG if their access tokens have Refresh Token / Expiry logic or if its the more basic Never Expire token style
- [ ] Double check with GGG if they have CORS on their Auth'd API endpoints.
