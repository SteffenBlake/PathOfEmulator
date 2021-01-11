# PathOfEmulator
A bare bones PathOfExile.com API emulator

See https://www.pathofexile.com/developer/docs/api-resources for documentation, though not all API endpoints are currently supported.

Currently supported endpoints:

`/oauth/authorize` OAuth 2.0 endpoint

`/token` OAuth 2.0 Access Token endpoint (pending, need confirmation on if this is the correct endpoint)

`/api/profile` Profile Endpoint, via Access Token

`/character-wondow/get-characters` Character list endpoint by Account name
