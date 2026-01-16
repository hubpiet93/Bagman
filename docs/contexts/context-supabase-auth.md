TITLE: Configure Supabase Auth API Environment Variables
DESCRIPTION: This snippet provides example environment variables for configuring the Supabase Auth (Gotrue) API server. It includes settings for host, port, external URL, and request ID headers.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_7

LANGUAGE: properties
CODE:
```
GOTRUE_API_HOST=localhost
PORT=9999
API_EXTERNAL_URL=http://localhost:9999
```

LANGUAGE: APIDOC
CODE:
```
API_HOST - string
  Hostname to listen on.
PORT (no prefix) / API_PORT - number
  Port number to listen on. Defaults to 8081.
API_ENDPOINT - string _Multi-instance mode only_
  Controls what endpoint Netlify can access this API on.
API_EXTERNAL_URL - string **required**
  The URL on which Gotrue might be accessed at.
REQUEST_ID_HEADER - string
  If you wish to inherit a request ID from the incoming request, specify the name in this value.
```

----------------------------------------

TITLE: Supabase Auth: List Users API (GET /admin/users)
DESCRIPTION: Details the API endpoint for retrieving a list of all users from Supabase Auth. It includes the HTTP GET request format, emphasizing the need for an admin JWT for authorization, and a comprehensive JSON response showing an array of user objects.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_22

LANGUAGE: http
CODE:
```
GET /admin/users HTTP/1.1
Host: localhost:9999
User-Agent: insomnia/2021.7.2
Authorization: Bearer <YOUR\*SIGNED_JWT>
Accept: */\_
```

LANGUAGE: json
CODE:
```
{
  "aud": "authenticated",
  "users": [
    {
      "id": "b7fd0253-6e16-4d4e-b61b-5943cb1b2102",
      "aud": "authenticated",
      "role": "authenticated",
      "email": "user+4@example.com",
      "phone": "",
      "app_metadata": {
        "provider": "email",
        "providers": [
          "email"
        ]
      },
      "user_metadata": {},
      "identities": null,
      "created_at": "2021-12-15T12:43:58.12207-05:00",
      "updated_at": "2021-12-15T12:43:58.122073-05:00"
    },
    {
      "id": "d69ae847-99be-4642-868f-439c2cdd9af4",
      "aud": "authenticated",
      "role": "authenticated",
      "email": "user+3@example.com",
      "phone": "",
      "app_metadata": {
        "provider": "email",
        "providers": [
          "email"
        ]
      },
      "user_metadata": {},
      "identities": null,
      "created_at": "2021-12-15T12:43:56.730209-05:00",
      "updated_at": "2021-12-15T12:43:56.730213-05:00"
    },
    {
      "id": "7282cf42-344e-4474-bdf6-d48e4968a2e4",
      "aud": "authenticated",
      "role": "authenticated",
      "email": "user+2@example.com",
      "phone": "",
      "app_metadata": {
        "provider": "email",
        "providers": [
          "email"
        ]
      },
      "user_metadata": {},
      "identities": null,
      "created_at": "2021-12-15T12:43:54.867676-05:00",
      "updated_at": "2021-12-15T12:43:54.867679-05:00"
    },
    {
      "id": "e78c512d-68e4-482b-901b-75003e89acae",
      "aud": "authenticated",
      "role": "authenticated",
      "email": "user@example.com",
      "phone": "",
      "app_metadata": {
        "provider": "email",
        "providers": [
          "email"
        ]
      },
      "user_metadata": {},
      "identities": null,
      "created_at": "2021-12-15T12:40:03.507551-05:00",
      "updated_at": "2021-12-15T12:40:03.507554-05:00"
    }
  ]
}
```

----------------------------------------

TITLE: Supabase Auth: Create User API (POST /admin/users)
DESCRIPTION: Documents the API endpoint for creating new users in Supabase Auth. This section provides the required JSON request body, the full HTTP request structure including necessary headers (like `Authorization` with an admin JWT), and a sample successful JSON response detailing the newly created user's properties.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_21

LANGUAGE: json
CODE:
```
{
  "email": "user@example.com",
  "password": "12345678"
}
```

LANGUAGE: http
CODE:
```
POST /admin/users HTTP/1.1
Host: localhost:9999
User-Agent: insomnia/2021.7.2
Content-Type: application/json
Authorization: Bearer <YOUR_SIGNED_JWT>
Accept: */*
Content-Length: 57
```

LANGUAGE: json
CODE:
```
{
  "id": "e78c512d-68e4-482b-901b-75003e89acae",
  "aud": "authenticated",
  "role": "authenticated",
  "email": "user@example.com",
  "phone": "",
  "app_metadata": {
    "provider": "email",
    "providers": [
      "email"
    ]
  },
  "user_metadata": {},
  "identities": null,
  "created_at": "2021-12-15T12:40:03.507551-05:00",
  "updated_at": "2021-12-15T12:40:03.512067-05:00"
}
```

----------------------------------------

TITLE: Supabase Auth GET /settings API Endpoint
DESCRIPTION: Returns the publicly available settings for the authentication instance, including enabled external providers and signup status. This endpoint provides insight into the configured authentication methods.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_25

LANGUAGE: APIDOC
CODE:
```
GET /settings
Description: Returns the publicly available settings for this auth instance.
Response Body (JSON):
{
  "external": {
    "apple": true,
    "azure": true,
    "bitbucket": true,
    "discord": true,
    "facebook": true,
    "figma": true,
    "github": true,
    "gitlab": true,
    "google": true,
    "keycloak": true,
    "linkedin": true,
    "notion": true,
    "slack": true,
    "snapchat":  true,
    "spotify": true,
    "twitch": true,
    "twitter": true,
    "workos": true
  },
  "disable_signup": false,
  "autoconfirm": false
}
```

----------------------------------------

TITLE: API Endpoint: GET /verify - Verify Registration or Recovery via Redirect
DESCRIPTION: Verifies a user's registration, password recovery, magic link, or invitation via a GET request with query parameters. The user is logged in and redirected to a specified URL with session details in the URL fragment. The 'type' parameter can be used to guide post-verification user experience.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_32

LANGUAGE: APIDOC
CODE:
```
GET /verify
```

LANGUAGE: json
CODE:
```
{
  "type": "signup",
  "token": "confirmation-code-delivered-in-email",
  "redirect_to": "https://supabase.io"
}
```

LANGUAGE: text
CODE:
```
SITE_URL/#access_token=jwt-token-representing-the-user&token_type=bearer&expires_in=3600&refresh_token=a-refresh-token&type=invite
```

----------------------------------------

TITLE: Start Supabase Auth Service Locally
DESCRIPTION: This command executes the compiled Supabase Auth binary, initiating the Auth service on your local machine. It will re-run migrations if necessary and then start the API, making it available for incoming requests.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_16

LANGUAGE: shell
CODE:
```
./auth
```

----------------------------------------

TITLE: Example Supabase Auth .env Configuration
DESCRIPTION: This snippet shows an example of a top-level configuration setting for Supabase Auth using a .env file. Environment variables prefixed with GOTRUE_ will always take precedence over values provided via file.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_5

LANGUAGE: properties
CODE:
```
GOTRUE_SITE_URL=https://example.netlify.com/
```

----------------------------------------

TITLE: API Endpoint: POST /invite - Invite New User
DESCRIPTION: Invites a new user via email. This endpoint requires a 'service_role' or 'supabase_admin' JWT in the Authorization Bearer header for authentication.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_30

LANGUAGE: APIDOC
CODE:
```
POST /invite
```

LANGUAGE: js
CODE:
```
headers: {
  "Authorization" : "Bearer eyJhbGciOiJI...M3A90LCkxxtX9oNP9KZO"
}
```

LANGUAGE: json
CODE:
```
{
  "email": "email@example.com"
}
```

LANGUAGE: json
CODE:
```
{
  "id": "11111111-2222-3333-4444-5555555555555",
  "email": "email@example.com",
  "confirmation_sent_at": "2016-05-15T20:49:40.882805774-07:00",
  "created_at": "2016-05-15T19:53:12.368652374-07:00",
  "updated_at": "2016-05-15T19:53:12.368652374-07:00",
  "invited_at": "2016-05-15T19:53:12.368652374-07:00"
}
```

----------------------------------------

TITLE: Configure Supabase Auth Logging Environment Variables
DESCRIPTION: This snippet provides example environment variables for configuring logging behavior in Supabase Auth (Gotrue). It includes settings for log level and specifying a log file path.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_10

LANGUAGE: properties
CODE:
```
LOG_LEVEL=debug # available without GOTRUE prefix (exception)
GOTRUE_LOG_FILE=/var/log/go/auth.log
```

LANGUAGE: APIDOC
CODE:
```
LOG_LEVEL - string
  Controls what log levels are output. Choose from panic, fatal, error, warn, info, or debug. Defaults to info.
LOG_FILE - string
  If you wish logs to be written to a file, set log_file to a valid file path.
```

----------------------------------------

TITLE: GET /authorize: Initiate OAuth Provider Authorization
DESCRIPTION: Initiates the OAuth flow with an external identity provider to obtain an access token. This endpoint redirects the user to the chosen provider for authentication, then back to the `/callback` endpoint. It supports various providers and allows for requesting additional scopes.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_41

LANGUAGE: text
CODE:
```
provider=apple | azure | bitbucket | discord | facebook | figma | github | gitlab | google | keycloak | linkedin | notion | slack | snapchat | spotify | twitch | twitter | workos

scopes=<optional additional scopes depending on the provider (email and name are requested by default)>
```

----------------------------------------

TITLE: Supabase Auth Settings Endpoint Response
DESCRIPTION: This JSON object details the configuration settings returned by the `/settings` endpoint of the Supabase Auth API. It provides comprehensive information on enabled external providers, signup options, and other service-specific configurations.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_19

LANGUAGE: json
CODE:
```
{
  "external": {
    "apple": false,
    "azure": false,
    "bitbucket": false,
    "discord": false,
    "github": false,
    "gitlab": false,
    "google": false,
    "facebook": false,
    "snapchat": false,
    "spotify": false,
    "slack": false,
    "slack_oidc": false,
    "twitch": true,
    "twitter": false,
    "email": true,
    "phone": false,
    "saml": false
  },
  "external_labels": {
    "saml": "auth0"
  },
  "disable_signup": false,
  "mailer_autoconfirm": false,
  "phone_autoconfirm": false,
  "sms_provider": "twilio"
}
```

----------------------------------------

TITLE: API Endpoint: POST /signup - Register New User
DESCRIPTION: Registers a new user using either an email and password or a phone number and password. Handles duplicate sign-ups by returning faux data to prevent information leakage, or an error if AUTOCONFIRM is enabled.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_28

LANGUAGE: APIDOC
CODE:
```
POST /signup
```

LANGUAGE: json
CODE:
```
{
  "email": "email@example.com",
  "password": "secret"
}
```

LANGUAGE: js
CODE:
```
{
  "id": "11111111-2222-3333-4444-5555555555555",
  "email": "email@example.com",
  "confirmation_sent_at": "2016-05-15T20:49:40.882805774-07:00",
  "created_at": "2016-05-15T19:53:12.368652374-07:00",
  "updated_at": "2016-05-15T19:53:12.368652374-07:00"
}

// if sign up is a duplicate then faux data will be returned
// as to not leak information about whether a given email
// has an account with your service or not
```

LANGUAGE: js
CODE:
```
{
  "phone": "12345678", // follows the E.164 format
  "password": "secret"
}
```

LANGUAGE: js
CODE:
```
{
  "id": "11111111-2222-3333-4444-5555555555555", // if duplicate sign up, this ID will be faux
  "phone": "12345678",
  "confirmation_sent_at": "2016-05-15T20:49:40.882805774-07:00",
  "created_at": "2016-05-15T19:53:12.368652374-07:00",
  "updated_at": "2016-05-15T19:53:12.368652374-07:00"
}
```

LANGUAGE: json
CODE:
```
{
  "code":400,
  "msg":"User already registered"
}
```

----------------------------------------

TITLE: Supabase Auth Service Startup Log Message
DESCRIPTION: This log message indicates that the Supabase Auth API has successfully started and is actively listening for connections. It confirms the service is operational and provides the network address where it can be accessed.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_17

LANGUAGE: terminal
CODE:
```
INFO[0000] Auth API started on: localhost:9999
```

----------------------------------------

TITLE: Configure Supabase Auth Database Environment Variables
DESCRIPTION: This snippet provides example environment variables for configuring the Supabase Auth (Gotrue) database connection. It includes settings for the database driver, connection URL, maximum pool size, and table namespace.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_8

LANGUAGE: properties
CODE:
```
GOTRUE_DB_DRIVER=postgres
DATABASE_URL=root@localhost/auth
```

LANGUAGE: APIDOC
CODE:
```
DB_DRIVER - string **required**
  Chooses what dialect of database you want. Must be postgres.
DATABASE_URL (no prefix) / DB_DATABASE_URL - string **required**
  Connection string for the database.
GOTRUE_DB_MAX_POOL_SIZE - int
  Sets the maximum number of open connections to the database. Defaults to 0 which is equivalent to an "unlimited" number of connections.
DB_NAMESPACE - string
  Adds a prefix to all table names.
```

----------------------------------------

TITLE: Start Supabase Auth Development Containers
DESCRIPTION: This command initiates the Supabase Auth development containers in an attached state, displaying log output directly in the terminal. It leverages Docker Compose to bring up necessary services like GoTrue and PostgreSQL.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_0

LANGUAGE: bash
CODE:
```
make dev
```

----------------------------------------

TITLE: Supabase Auth POST/PUT /admin/users/<user_id> API Endpoint
DESCRIPTION: Allows for the creation (POST) or update (PUT) of a user based on the provided user_id. This endpoint supports setting user roles, email, phone, password, metadata, and managing ban durations with specific time units.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_26

LANGUAGE: APIDOC
CODE:
```
POST, PUT /admin/users/<user_id>
Description: Creates (POST) or Updates (PUT) the user based on the user_id specified. The ban_duration field accepts the following time units: "ns", "us", "ms", "s", "m", "h". See time.ParseDuration for more details on the format used.
Request Headers:
  Authorization: Bearer <JWT_TOKEN> (requires a role claim that can be set in the GOTRUE_JWT_ADMIN_ROLES env var)
Request Body:
{
  "role": "test-user",
  "email": "email@example.com",
  "phone": "12345678",
  "password": "secret", // only if type = signup
  "email_confirm": true,
  "phone_confirm": true,
  "user_metadata": {},
  "app_metadata": {},
  "ban_duration": "24h" or "none" // to unban a user
}
```

----------------------------------------

TITLE: GET /user: Retrieve Authenticated User Data
DESCRIPTION: Retrieves the JSON object containing details for the currently logged-in user. This endpoint requires successful authentication, typically via an Authorization Bearer token in the request headers.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_37

LANGUAGE: json
CODE:
```
{
  "id": "11111111-2222-3333-4444-5555555555555",
  "email": "email@example.com",
  "confirmation_sent_at": "2016-05-15T20:49:40.882805774-07:00",
  "created_at": "2016-05-15T19:53:12.368652374-07:00",
  "updated_at": "2016-05-15T19:53:12.368652374-07:00"
}
```

----------------------------------------

TITLE: Verify Running Supabase Auth Docker Containers
DESCRIPTION: Lists all currently running Docker containers to confirm that both the `auth_postgresql` and `gotrue_gotrue` services are active and operational after starting the Docker development environment.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_4

LANGUAGE: bash
CODE:
```
docker ps
```

----------------------------------------

TITLE: Run Supabase Auth Development with Docker
DESCRIPTION: Executes the `make dev` command to start Supabase Auth and its associated services within Docker containers. This provides a fully containerized development environment.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_3

LANGUAGE: bash
CODE:
```
make dev
```

----------------------------------------

TITLE: API Endpoint: POST /resend - Resend OTP
DESCRIPTION: Allows a user to resend an existing signup, SMS, email change, or phone change One-Time Password (OTP). Requires either an email or phone number and the type of OTP to resend.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_29

LANGUAGE: APIDOC
CODE:
```
POST /resend
```

LANGUAGE: json
CODE:
```
{
  "email": "user@example.com",
  "type": "signup"
}
```

LANGUAGE: json
CODE:
```
{
  "phone": "12345678",
  "type": "sms"
}
```

LANGUAGE: json
CODE:
```
{
  "message_id": "msgid123456"
}
```

----------------------------------------

TITLE: Supabase Auth Database Connection String Example
DESCRIPTION: This environment variable defines the connection string for Supabase Auth to establish a connection with the PostgreSQL database. It specifies the user, password, host, port, and database name, enabling Auth to communicate with its data store.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_15

LANGUAGE: env
CODE:
```
DATABASE_URL="postgres://supabase_auth_admin:root@localhost:5432/postgres"
```

----------------------------------------

TITLE: Supabase Auth Health Endpoint Response
DESCRIPTION: This JSON object represents the expected response from the `/health` endpoint of the Supabase Auth API. It provides basic information about the service, including its description, name, and version, confirming its availability and purpose.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_18

LANGUAGE: json
CODE:
```
{
  "description": "Auth is a user registration and authentication API",
  "name": "Auth",
  "version": ""
}
```

----------------------------------------

TITLE: GET /callback: Handle OAuth Provider Redirect
DESCRIPTION: This endpoint serves as the redirect URI for external OAuth providers after successful authentication. It processes the provider's response and redirects the user back to the configured site URL, including access_token, refresh_token, and other relevant OAuth parameters.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_42

LANGUAGE: APIDOC
CODE:
```
GET /callback
  Description: External OAuth provider redirect URI. Redirects to <GOTRUE_SITE_URL> with access_token, refresh_token, provider_token, expires_in, and provider_name.
```

----------------------------------------

TITLE: GoTrue Email Configuration Variables Reference
DESCRIPTION: Detailed API documentation for environment variables used to configure email functionality in Supabase's GoTrue service. Includes SMTP settings, email frequency controls, sender names, auto-confirmation, OTP expiration, URL paths for various email types, and subject lines.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_18

LANGUAGE: APIDOC
CODE:
```
SMTP_ADMIN_EMAIL: string (required)
  Description: The "From" email address for all emails sent.
SMTP_HOST: string (required)
  Description: The mail server hostname to send emails through.
SMTP_PORT: number (required)
  Description: The port number to connect to the mail server on.
SMTP_USER: string
  Description: If the mail server requires authentication, the username to use.
SMTP_PASS: string
  Description: If the mail server requires authentication, the password to use.
SMTP_MAX_FREQUENCY: number
  Description: Controls the minimum amount of time that must pass before sending another signup confirmation or password reset email. The value is the number of seconds. Defaults to 900 (15 minutes).
SMTP_SENDER_NAME: string
  Description: Sets the name of the sender. Defaults to the SMTP_ADMIN_EMAIL if not used.
MAILER_AUTOCONFIRM: bool
  Description: If you do not require email confirmation, you may set this to true. Defaults to false.
MAILER_OTP_EXP: number
  Description: Controls the duration an email link or otp is valid for.
MAILER_URLPATHS_INVITE: string
  Description: URL path to use in the user invite email. Defaults to /verify.
MAILER_URLPATHS_CONFIRMATION: string
  Description: URL path to use in the signup confirmation email. Defaults to /verify.
MAILER_URLPATHS_RECOVERY: string
  Description: URL path to use in the password reset email. Defaults to /verify.
MAILER_URLPATHS_EMAIL_CHANGE: string
  Description: URL path to use in the email change confirmation email. Defaults to /verify.
MAILER_SUBJECTS_INVITE: string
  Description: Email subject to use for user invite. Defaults to You have been invited.
MAILER_SUBJECTS_CONFIRMATION: string
  Description: Email subject to use for signup confirmation. Defaults to Confirm Your Signup.
MAILER_SUBJECTS_RECOVERY: string
  Description: Email subject to use for password reset. Defaults to Reset Your Password.
MAILER_SUBJECTS_MAGIC_LINK: string
  Description: Email subject to use for magic link email. Defaults to Your Magic Link.
MAILER_SUBJECTS_EMAIL_CHANGE: string
  Description: Email subject to use for email change confirmation. Defaults to Confirm Email Change.
MAILER_TEMPLATES_INVITE: string
  Description: URL path to an email template to use when inviting a user. (e.g. https://www.example.com/path-to-email-template.html)
MAILER_TEMPLATES_CONFIRMATION: string
  Description: URL path to an email template to use when confirming a signup. (e.g. https://www.example.com/path-to-email-template.html)
MAILER_TEMPLATES_RECOVERY: string
  Description: URL path to an email template to use when resetting a password. (e.g. https://www.example.com/path-to-email-template.html)
MAILER_TEMPLATES_MAGIC_LINK: string
  Description: URL path to an email template to use when sending magic link. (e.g. https://www.example.com/path-to-email-template.html)
MAILER_TEMPLATES_EMAIL_CHANGE: string
  Description: URL path to an email template to use when confirming the change of an email address. (e.g. https://www.example.com/path-to-email-template.html)
```

----------------------------------------

TITLE: Execute Supabase Auth Server Locally
DESCRIPTION: Runs the compiled Supabase Auth server binary directly from the current directory. This command is used after building the binary and starting the PostgreSQL database.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_2

LANGUAGE: bash
CODE:
```
./auth
```

----------------------------------------

TITLE: Supabase Auth POST /admin/generate_link API Endpoint
DESCRIPTION: Generates and returns an email action link based on the specified type (signup, magiclink, recovery, invite). The response includes the action link, email OTP, hashed token, verification type, and redirect URL for convenience.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_27

LANGUAGE: APIDOC
CODE:
```
POST /admin/generate_link
Description: Returns the corresponding email action link based on the type specified. Among other things, the response also contains the query params of the action link as separate JSON fields for convenience (along with the email OTP from which the corresponding token is generated).
Request Headers:
  Authorization: Bearer <JWT_TOKEN> (admin role required)
Request Body:
{
  "type": "signup" or "magiclink" or "recovery" or "invite",
  "email": "email@example.com",
  "password": "secret", // only if type = signup
  "data": {
    ...
  }, // only if type = signup
  "redirect_to": "https://supabase.io" // Redirect URL to send the user to after an email action. Defaults to SITE_URL.
}
Response Body:
{
  "action_link": "http://localhost:9999/verify?token=TOKEN&type=TYPE&redirect_to=REDIRECT_URL",
  "email_otp": "EMAIL_OTP",
  "hashed_token": "TOKEN",
  "verification_type": "TYPE",
  "redirect_to": "REDIRECT_URL",
  ...
}
```

----------------------------------------

TITLE: Build Supabase Auth Binary
DESCRIPTION: Compiles the Supabase Auth server using Go, setting version information from Git and creating an ARM64 executable for Linux. This command is part of the manual quick start process.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_0

LANGUAGE: bash
CODE:
```
go build -ldflags "-X github.com/supabase/auth/cmd.Version=`git rev-parse HEAD`"
GOOS=linux GOARCH=arm64 go build -ldflags "-X github.com/supabase/auth/cmd.Version=`git rev-parse HEAD`" -o gotrue-arm64
```

----------------------------------------

TITLE: API Endpoint: POST /otp - Generate One-Time Password
DESCRIPTION: Delivers a magic link or SMS OTP to the user based on whether an 'email' or 'phone' key is present in the request body. If 'create_user' is true, a new user will not be automatically signed up if they don't exist.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_33

LANGUAGE: APIDOC
CODE:
```
POST /otp
```

LANGUAGE: js
CODE:
```
{
  "phone": "12345678" // follows the E.164 format
  "create_user": true
}
```

LANGUAGE: js
CODE:
```
// exactly the same as /magiclink
{
  "email": "email@example.com"
  "create_user": true
}
```

LANGUAGE: json
CODE:
```
{}
```

----------------------------------------

TITLE: API Endpoint: POST /verify - Verify Registration or Recovery
DESCRIPTION: Verifies a user's registration, password recovery, or invitation using a token. Supports email-based signup/recovery/invite and phone-based SMS OTP verification. A password may be required for signup verification if no existing password exists.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_31

LANGUAGE: APIDOC
CODE:
```
POST /verify
```

LANGUAGE: json
CODE:
```
{
  "type": "signup",
  "token": "confirmation-code-delivered-in-email"
}
```

LANGUAGE: json
CODE:
```
{
  "access_token": "jwt-token-representing-the-user",
  "token_type": "bearer",
  "expires_in": 3600,
  "refresh_token": "a-refresh-token",
  "type": "signup | recovery | invite"
}
```

LANGUAGE: json
CODE:
```
{
  "type": "sms",
  "token": "confirmation-otp-delivered-in-sms",
  "redirect_to": "https://supabase.io",
  "phone": "phone-number-sms-otp-was-delivered-to"
}
```

LANGUAGE: json
CODE:
```
{
  "access_token": "jwt-token-representing-the-user",
  "token_type": "bearer",
  "expires_in": 3600,
  "refresh_token": "a-refresh-token"
}
```

----------------------------------------

TITLE: GET /reauthenticate: Request Reauthentication Nonce
DESCRIPTION: Sends a nonce to the user's registered email or phone number for reauthentication purposes. This endpoint requires the user to be currently logged in and to have either an email or phone number associated with their account.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_39

LANGUAGE: js
CODE:
```
headers: {
  "Authorization" : "Bearer eyJhbGciOiJI...M3A90LCkxxtX9oNP9KZO"
}
```

----------------------------------------

TITLE: Start Local PostgreSQL for Supabase Auth Development
DESCRIPTION: Initiates a local PostgreSQL database instance using Docker Compose, specifically configured for the development environment of Supabase Auth. This is the first step for running Auth without a full Docker setup.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_1

LANGUAGE: bash
CODE:
```
docker-compose -f docker-compose-dev.yml up postgres
```

----------------------------------------

TITLE: Configure GoTrue SMTP Environment Variables
DESCRIPTION: Example environment variables for setting up SMTP email delivery within the Supabase GoTrue authentication service. These variables control the mail server host, port, authentication credentials, and admin email.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_17

LANGUAGE: properties
CODE:
```
GOTRUE_SMTP_HOST=smtp.mandrillapp.com
GOTRUE_SMTP_PORT=587
GOTRUE_SMTP_USER=smtp-delivery@example.com
GOTRUE_SMTP_PASS=correcthorsebatterystaple
GOTRUE_SMTP_ADMIN_EMAIL=support@example.com
GOTRUE_MAILER_SUBJECTS_CONFIRMATION="Please confirm"
```

----------------------------------------

TITLE: Generate Supabase Admin JWT Payload
DESCRIPTION: Illustrates the minimal JSON payload structure required to generate a JSON Web Token (JWT) with the `supabase_admin` role. This JWT is essential for authenticating requests to Supabase's admin API endpoints.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_20

LANGUAGE: json
CODE:
```
{
  "role": "supabase_admin"
}
```

----------------------------------------

TITLE: POST /token: OAuth2 Token Grant Endpoint
DESCRIPTION: This OAuth2 endpoint supports both password and refresh_token grant types. Users can obtain access and refresh tokens by providing email/password or phone/password credentials, or by exchanging an existing refresh token. The returned access token is used for subsequent authenticated API requests.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_36

LANGUAGE: text
CODE:
```
?grant_type=password
```

LANGUAGE: js
CODE:
```
// Email login
{
  "email": "name@domain.com",
  "password": "somepassword"
}

// Phone login
{
  "phone": "12345678",
  "password": "somepassword"
}
```

LANGUAGE: text
CODE:
```
grant_type=refresh_token
```

LANGUAGE: json
CODE:
```
{
  "refresh_token": "a-refresh-token"
}
```

LANGUAGE: json
CODE:
```
{
  "access_token": "jwt-token-representing-the-user",
  "token_type": "bearer",
  "expires_in": 3600,
  "refresh_token": "a-refresh-token"
}
```

----------------------------------------

TITLE: Supabase Auth Configuration Parameters Reference
DESCRIPTION: This section details the various configuration parameters available for Supabase Auth, which can be set via environment variables (prefixed with GOTRUE_) or a .env file. It includes descriptions, types, and default values for each setting.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_6

LANGUAGE: APIDOC
CODE:
```
SITE_URL: string (required)
  Description: The base URL your site is located at. Currently used in combination with other settings to construct URLs used in emails. Any URI that shares a host with SITE_URL is a permitted value for redirect_to params (see /authorize etc.).
URI_ALLOW_LIST: string
  Description: A comma separated list of URIs (e.g. "https://foo.example.com,https://*.foo.example.com,https://bar.example.com") which are permitted as valid redirect_to destinations. Defaults to []. Supports wildcard matching through globbing. e.g. https://*.foo.example.com will allow https://a.foo.example.com and https://b.foo.example.com to be accepted. Globbing is also supported on subdomains. e.g. https://foo.example.com/* will allow https://foo.example.com/page1 and https://foo.example.com/page2 to be accepted.
OPERATOR_TOKEN: string (Multi-instance mode only)
  Description: The shared secret with an operator (usually Netlify) for this microservice. Used to verify requests have been proxied through the operator and the payload values can be trusted.
DISABLE_SIGNUP: bool
  Description: When signup is disabled the only way to create new users is through invites. Defaults to false, all signups enabled.
GOTRUE_EXTERNAL_EMAIL_ENABLED: bool
  Description: Use this to disable email signups (users can still use external oauth providers to sign up / sign in)
GOTRUE_EXTERNAL_PHONE_ENABLED: bool
  Description: Use this to disable phone signups (users can still use external oauth providers to sign up / sign in)
GOTRUE_RATE_LIMIT_HEADER: string
  Description: Header on which to rate limit the /token endpoint.
GOTRUE_RATE_LIMIT_EMAIL_SENT: string
  Description: Rate limit the number of emails sent per hr on the following endpoints: /signup, /invite, /magiclink, /recover, /otp, & /user.
GOTRUE_PASSWORD_MIN_LENGTH: int
  Description: Minimum password length, defaults to 6.
GOTRUE_PASSWORD_REQUIRED_CHARACTERS: string
  Description: A string of character sets separated by :. A password must contain at least one character of each set to be accepted. To use the : character escape it with \.
GOTRUE_SECURITY_REFRESH_TOKEN_ROTATION_ENABLED: bool
  Description: If refresh token rotation is enabled, auth will automatically detect malicious attempts to reuse a revoked refresh token. When a malicious attempt is detected, gotrue immediately revokes all tokens that descended from the offending token.
GOTRUE_SECURITY_REFRESH_TOKEN_REUSE_INTERVAL: string
  Description: This setting is only applicable if GOTRUE_SECURITY_REFRESH_TOKEN_ROTATION_ENABLED is enabled. The reuse interval for a refresh token allows for exchanging the refresh token multiple times during the interval to support concurrency or offline issues. During the reuse interval, auth will not consider using a revoked token as a malicious attempt and will simply return the child refresh token. Only the previous revoked token can be reused. Using an old refresh token way before the current valid refresh token will trigger the reuse detection.
```

----------------------------------------

TITLE: Configure OpenTelemetry Debugging and Custom Resource Attributes
DESCRIPTION: This snippet provides environment variables for debugging OpenTelemetry issues and defining custom resource attributes for traces and metrics in Supabase Auth. It highlights the standard `DEBUG` and `OTEL_RESOURCE_ATTRIBUTES` variables.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_13

LANGUAGE: APIDOC
CODE:
```
DEBUG - boolean
  Set to true to get more insights from the OpenTelemetry SDK.
OTEL_RESOURCE_ATTRIBUTES - string
  Standard OpenTelemetry environment variable for defining custom resource attributes.
```

LANGUAGE: Shell
CODE:
```
DEBUG=true
```

----------------------------------------

TITLE: Configure Supabase Auth OpenTelemetry Tracing
DESCRIPTION: This snippet details environment variables for enabling and configuring OpenTelemetry tracing in Supabase Auth. It includes general tracing settings and specific OTLP exporter configurations for services like Honeycomb.io.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_11

LANGUAGE: APIDOC
CODE:
```
GOTRUE_TRACING_ENABLED - boolean
GOTRUE_TRACING_EXPORTER - string only opentelemetry supported
```

LANGUAGE: Shell
CODE:
```
OTEL_SERVICE_NAME=auth
OTEL_EXPORTER_OTLP_PROTOCOL=grpc
OTEL_EXPORTER_OTLP_ENDPOINT=https://api.honeycomb.io:443
OTEL_EXPORTER_OTLP_HEADERS="x-honeycomb-team=<API-KEY>,x-honeycomb-dataset=auth"
```

----------------------------------------

TITLE: Supabase Auth: Database Setup and Testing Commands (Shell)
DESCRIPTION: Outlines the shell commands for setting up the database using Docker Compose, applying migrations with `make migrate_test`, and executing tests for the Supabase Auth service. It notes that the same database is used for both local development and testing.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_24

LANGUAGE: sh
CODE:
```
# Runs the database in a docker container
$ docker-compose -f docker-compose-dev.yml up postgres
```

LANGUAGE: sh
CODE:
```
# Applies the migrations to the database (requires soda cli)
$ make migrate_test
```

LANGUAGE: sh
CODE:
```
# Executes the tests
$ make test
```

----------------------------------------

TITLE: Configure Supabase Auth OpenTelemetry Metrics
DESCRIPTION: This snippet details environment variables for enabling and configuring OpenTelemetry metrics in Supabase Auth. It covers general metrics settings, Prometheus exporter options, and OTLP exporter configurations for services like Honeycomb.io.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_12

LANGUAGE: APIDOC
CODE:
```
GOTRUE_METRICS_ENABLED - boolean
GOTRUE_METRICS_EXPORTER - string only opentelemetry and prometheus supported
OTEL_EXPORTER_PROMETHEUS_HOST - IP address, default 0.0.0.0
OTEL_EXPORTER_PROMETHEUS_PORT - port number, default 9100
```

LANGUAGE: Shell
CODE:
```
OTEL_SERVICE_NAME=auth
OTEL_EXPORTER_OTLP_PROTOCOL=grpc
OTEL_EXPORTER_OTLP_ENDPOINT=https://api.honeycomb.io:443
OTEL_EXPORTER_OTLP_HEADERS="x-honeycomb-team=<API-KEY>,x-honeycomb-dataset=auth"
```

----------------------------------------

TITLE: Supabase Auth Environment Variable Configuration
DESCRIPTION: Details the environment variables used to configure various aspects of Supabase Auth, including phone authentication settings, CAPTCHA integration, reauthentication requirements, and anonymous sign-in capabilities.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_24

LANGUAGE: APIDOC
CODE:
```
### Phone Auth
SMS_AUTOCONFIRM: bool
  If you do not require phone confirmation, you may set this to true. Defaults to false.
SMS_MAX_FREQUENCY: number
  Controls the minimum amount of time that must pass before sending another sms otp. The value is the number of seconds. Defaults to 60 (1 minute).
SMS_OTP_EXP: number
  Controls the duration an sms otp is valid for.
SMS_OTP_LENGTH: number
  Controls the number of digits of the sms otp sent.
SMS_PROVIDER: string
  Available options are: twilio, messagebird, textlocal, and vonage
SMS_TWILIO_ACCOUNT_SID: string
SMS_TWILIO_AUTH_TOKEN: string
SMS_TWILIO_MESSAGE_SERVICE_SID: string
  Can be set to your twilio sender mobile number.
SMS_MESSAGEBIRD_ACCESS_KEY: string
  Your Messagebird access key.
SMS_MESSAGEBIRD_ORIGINATOR: string
  SMS sender (your Messagebird phone number with + or company name).

### CAPTCHA
SECURITY_CAPTCHA_ENABLED: string
  Whether captcha middleware is enabled.
SECURITY_CAPTCHA_PROVIDER: string
  For now the only options supported are: hcaptcha and turnstile.
SECURITY_CAPTCHA_SECRET: string
SECURITY_CAPTCHA_TIMEOUT: string
  Retrieve from hcaptcha or turnstile account.

### Reauthentication
SECURITY_UPDATE_PASSWORD_REQUIRE_REAUTHENTICATION: bool
  Enforce reauthentication on password update.

### Anonymous Sign-Ins
GOTRUE_EXTERNAL_ANONYMOUS_USERS_ENABLED: bool
  Use this to enable/disable anonymous sign-ins.
```

----------------------------------------

TITLE: Run Supabase Auth Database Migrations
DESCRIPTION: These commands demonstrate how to manually run database migrations for Supabase Auth. Options are provided for both locally built binaries and Docker container usage.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_9

LANGUAGE: Shell
CODE:
```
./auth migrate
```

LANGUAGE: Shell
CODE:
```
docker run --rm auth gotrue migrate
```

----------------------------------------

TITLE: Build Supabase Auth Binary
DESCRIPTION: This `make` command compiles the Supabase Auth source code into an executable binary. It is a prerequisite for running the Auth service locally and ensures that the latest changes are incorporated into the application.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_12

LANGUAGE: make
CODE:
```
make build
```

----------------------------------------

TITLE: Manage Supabase Auth Docker Containers
DESCRIPTION: Essential Docker commands for interacting with the `auth_postgres` container, including inspecting its configuration, executing commands inside it, and removing containers and volumes.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_26

LANGUAGE: YAML
CODE:
```
container_name: auth_postgres
```

LANGUAGE: Zsh
CODE:
```
# Command line into bash on the PostgreSQL container
docker exec -it auth_postgres bash

# Removes Container
docker container rm -f auth_postgres

# Removes volume
docker volume rm postgres_data
```

----------------------------------------

TITLE: Configure External Authentication Providers
DESCRIPTION: This section describes how to enable and configure various external OAuth providers for Supabase Auth, such as GitHub. It specifies the required environment variables for client ID, client secret, and redirect URI, along with an optional base URL for certain providers.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_15

LANGUAGE: properties
CODE:
```
GOTRUE_EXTERNAL_GITHUB_ENABLED=true
GOTRUE_EXTERNAL_GITHUB_CLIENT_ID=myappclientid
GOTRUE_EXTERNAL_GITHUB_SECRET=clientsecretvaluessssh
GOTRUE_EXTERNAL_GITHUB_REDIRECT_URI=http://localhost:3000/callback
```

LANGUAGE: APIDOC
CODE:
```
EXTERNAL_X_ENABLED: bool
  Description: Whether this external provider is enabled or not.
EXTERNAL_X_CLIENT_ID: string (required)
  Description: The OAuth2 Client ID registered with the external provider.
EXTERNAL_X_SECRET: string (required)
  Description: The OAuth2 Client Secret provided by the external provider when you registered.
EXTERNAL_X_REDIRECT_URI: string (required)
  Description: The URI a OAuth2 provider will redirect to with the 'code' and 'state' values.
EXTERNAL_X_URL: string
  Description: The base URL used for constructing the URLs to request authorization and access tokens. Used by 'gitlab' and 'keycloak'. For 'gitlab' it defaults to 'https://gitlab.com'. For 'keycloak' you need to set this to your instance, for example: 'https://keycloak.example.com/realms/myrealm'
```

----------------------------------------

TITLE: Modify Go Server for Local HTTPS with Apple OAuth
DESCRIPTION: This Go code snippet demonstrates how to modify the `ListenAndServe` function in `api.go` to enable HTTPS traffic. This modification is crucial for local development and testing of external authentication providers like Apple OAuth, which often require secure connections.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_16

LANGUAGE: Go
CODE:
```
func (a *API) ListenAndServe(hostAndPort string) {
  log := logrus.WithField("component", "api")
  path, err := os.Getwd()
  if err != nil {
    log.Println(err)
  }
  server := &http.Server{
    Addr:    hostAndPort,
    Handler: a.handler,
  }
  done := make(chan struct{})
  defer close(done)
  go func() {
    waitForTermination(log, done)
    ctx, cancel := context.WithTimeout(context.Background(), time.Minute)
    defer cancel()
    server.Shutdown(ctx)
  }()
  if err := server.ListenAndServeTLS("PATH_TO_CRT_FILE", "PATH_TO_KEY_FILE"); err != http.ErrServerClosed {
    log.WithError(err).Fatal("http server listen failed")
  }
}
```

----------------------------------------

TITLE: Apply Supabase Auth Database Migrations with Soda
DESCRIPTION: This `make` command executes the database migrations for the Supabase Auth service using Soda. It sets up the necessary schema in the PostgreSQL database, ensuring the application has the correct table structure to function properly.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_13

LANGUAGE: make
CODE:
```
make migrate_test
```

----------------------------------------

TITLE: Synchronize Git Tags for Supabase Auth Repository
DESCRIPTION: These Git commands ensure that all tags from the upstream repository are fetched and then pushed to your local fork. This step is necessary to correctly build the Supabase Auth binary, as tags are not automatically copied during a repository fork operation.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_11

LANGUAGE: shell
CODE:
```
# Fetch the tags from the upstream repository
git fetch upstream --tags

# Push the tags to your fork
git push origin --tags
```

----------------------------------------

TITLE: Default HTML Template for User Invite Email
DESCRIPTION: HTML content for the default email template used when inviting a new user. This template includes placeholders for SiteURL, Email, and ConfirmationURL.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_19

LANGUAGE: html
CODE:
```
<h2>You have been invited</h2>

<p>
  You have been invited to create a user on {{ .SiteURL }}. Follow this link to
  accept the invite:
</p>
<p><a href="{{ .ConfirmationURL }}">Accept the invite</a></p>
```

----------------------------------------

TITLE: POST /recover: Initiate Password Recovery
DESCRIPTION: Sends a password recovery email to the user's email address. This endpoint is rate-limited, allowing recovery links to be sent only once every 60 seconds.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_35

LANGUAGE: json
CODE:
```
{
  "email": "email@example.com"
}
```

LANGUAGE: json
CODE:
```
{}
```

----------------------------------------

TITLE: Clone Supabase Auth Git Repository
DESCRIPTION: This command clones the official Supabase Auth Git repository from GitHub. It is the initial step required to obtain the project's source code and begin local development or contribution.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_7

LANGUAGE: zsh
CODE:
```
git clone https://github.com/supabase/auth
```

----------------------------------------

TITLE: Run Supabase Auth PostgreSQL Container with Docker Compose
DESCRIPTION: This command initiates the PostgreSQL database container essential for Supabase Auth's operation using Docker Compose. It leverages the `docker-compose-dev.yml` file to define the service, making the database accessible on port 5432 for local development.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_9

LANGUAGE: docker-compose
CODE:
```
docker-compose -f docker-compose-dev.yml up postgres
```

----------------------------------------

TITLE: Update Go Module Dependencies
DESCRIPTION: Commands to update project dependencies, specifically using `make deps` and `go mod tidy` for Go modules.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_27

LANGUAGE: Shell
CODE:
```
make deps
```

LANGUAGE: Shell
CODE:
```
go mod tidy
```

----------------------------------------

TITLE: Build PostgreSQL Docker Image for Supabase Auth
DESCRIPTION: This command builds the PostgreSQL Docker image specifically configured for the Supabase Auth development environment. It utilizes the `docker-compose-dev.yml` file to define the service and its build context, ensuring proper database setup.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_8

LANGUAGE: zsh
CODE:
```
docker-compose -f docker-compose-dev.yml build postgres
```

----------------------------------------

TITLE: Default HTML Template for Signup Confirmation Email
DESCRIPTION: HTML content for the default email template used to confirm a user's signup. This template includes placeholders for SiteURL, Email, and ConfirmationURL.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_20

LANGUAGE: html
CODE:
```
<h2>Confirm your signup</h2>

<p>Follow this link to confirm your user:</p>
<p><a href="{{ .ConfirmationURL }}">Confirm your mail</a></p>
```

----------------------------------------

TITLE: POST /magiclink: Send Magic Link for Authentication
DESCRIPTION: Sends a magic link to the user's email address for authentication. This endpoint is rate-limited to once every 60 seconds. Upon clicking, the magic link redirects the user to a verification URL containing access and refresh tokens.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_34

LANGUAGE: json
CODE:
```
{
  "email": "email@example.com"
}
```

LANGUAGE: json
CODE:
```
{}
```

----------------------------------------

TITLE: Run Supabase Auth Tests in Docker Containers
DESCRIPTION: Execute this command to run the project's test suite within the Docker containers. It ensures a fresh database state before initiating the tests, providing a clean and reliable environment for test execution.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_1

LANGUAGE: bash
CODE:
```
make docker-test
```

----------------------------------------

TITLE: Configure JSON Web Token (JWT) Settings
DESCRIPTION: This section details the environment variables used to configure JSON Web Tokens (JWTs) within the Supabase Auth system. It covers setting the secret for signing tokens, their validity period, the default audience, and names for admin and default user groups.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_14

LANGUAGE: properties
CODE:
```
GOTRUE_JWT_SECRET=supersecretvalue
GOTRUE_JWT_EXP=3600
GOTRUE_JWT_AUD=netlify
```

LANGUAGE: APIDOC
CODE:
```
JWT_SECRET: string (required)
  Description: The secret used to sign JWT tokens with.
JWT_EXP: number
  Description: How long tokens are valid for, in seconds. Defaults to 3600 (1 hour).
JWT_AUD: string
  Description: The default JWT audience. Use audiences to group users.
JWT_ADMIN_GROUP_NAME: string
  Description: The name of the admin group (if enabled). Defaults to 'admin'.
JWT_DEFAULT_GROUP_NAME: string
  Description: The default group to assign all new users to.
```

----------------------------------------

TITLE: Install Docker via Homebrew on macOS
DESCRIPTION: This command installs Docker using Homebrew on macOS. Docker is a fundamental tool for running the Supabase Auth development containers and managing the project's various services.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_5

LANGUAGE: zsh
CODE:
```
brew install docker
```

----------------------------------------

TITLE: Supabase Auth: Run Database Migrations (Zsh)
DESCRIPTION: Provides the Zsh command `make migrate_test` used to apply database migrations for the Supabase Auth service. This command ensures the database schema is up-to-date with the latest changes.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_23

LANGUAGE: zsh
CODE:
```
make migrate_test
```

----------------------------------------

TITLE: Install Soda CLI via Homebrew on macOS
DESCRIPTION: This command installs the Soda CLI, a database toolbox used for managing database schema and migrations, via Homebrew on macOS. It is a crucial dependency for interacting with the PostgreSQL database in the Supabase Auth project.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_6

LANGUAGE: zsh
CODE:
```
brew install gobuffalo/tap/pop
```

----------------------------------------

TITLE: PUT /user: Update User Information
DESCRIPTION: Updates the authenticated user's profile, allowing changes to email, password, phone number, and custom user data. Changing the email address will trigger a magic link to be sent for verification. This endpoint requires prior authentication.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_38

LANGUAGE: json
CODE:
```
{
  "email": "new-email@example.com",
  "password": "new-password",
  "phone": "+123456789",
  "data": {
    "key": "value",
    "number": 10,
    "admin": false
  }
}
```

LANGUAGE: json
CODE:
```
{
  "id": "11111111-2222-3333-4444-5555555555555",
  "email": "email@example.com",
  "email_change_sent_at": "2016-05-15T20:49:40.882805774-07:00",
  "phone": "+123456789",
  "phone_change_sent_at": "2016-05-15T20:49:40.882805774-07:00",
  "created_at": "2016-05-15T19:53:12.368652374-07:00",
  "updated_at": "2016-05-15T19:53:12.368652374-07:00"
}
```

LANGUAGE: json
CODE:
```
{
  "password": "new-password",
  "nonce": "123456"
}
```

----------------------------------------

TITLE: POST /logout: Log Out User and Revoke Tokens
DESCRIPTION: Logs out the authenticated user by revoking all associated refresh tokens. Note that existing JWT access tokens will remain valid until their natural expiration, as this is a stateless authentication mechanism.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_40

LANGUAGE: APIDOC
CODE:
```
POST /logout
  Description: Logs out a user by revoking all refresh tokens. JWTs remain valid until expiration.
```

----------------------------------------

TITLE: Default HTML Template for Magic Link Email
DESCRIPTION: HTML content for the default email template used to send a magic link for login. This template includes placeholders for SiteURL, Email, and ConfirmationURL.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_22

LANGUAGE: html
CODE:
```
<h2>Magic Link</h2>

<p>Follow this link to login:</p>
<p><a href="{{ .ConfirmationURL }}">Log In</a></p>
```

----------------------------------------

TITLE: Default HTML Template for Email Change Confirmation
DESCRIPTION: HTML content for the default email template used to confirm an email address change. This template includes placeholders for SiteURL, Email, NewEmail, and ConfirmationURL.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_23

LANGUAGE: html
CODE:
```
<h2>Confirm Change of Email</h2>

<p>
  Follow this link to confirm the update of your email from {{ .Email }} to {{
  .NewEmail }}:
</p>
<p><a href="{{ .ConfirmationURL }}">Change Email</a></p>
```

----------------------------------------

TITLE: Default HTML Template for Password Recovery Email
DESCRIPTION: HTML content for the default email template used for password reset requests. This template includes placeholders for SiteURL, Email, and ConfirmationURL.
SOURCE: https://github.com/supabase/auth/blob/master/README.md#_snippet_21

LANGUAGE: html
CODE:
```
<h2>Reset Password</h2>

<p>Follow this link to reset the password for your user:</p>
<p><a href="{{ .ConfirmationURL }}">Reset Password</a></p>
```

----------------------------------------

TITLE: Remove Supabase Auth Docker Containers and Volumes
DESCRIPTION: Use this command to stop and remove all Supabase Auth Docker containers and their associated volumes. This action permanently deletes any data linked to the containers, providing a clean slate for future development.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_2

LANGUAGE: bash
CODE:
```
make docker-clean
```

----------------------------------------

TITLE: Supabase Auth Database Migration Success Log Output
DESCRIPTION: This terminal output displays the successful application of Supabase Auth database migrations. It lists the SQL queries executed and the status of each migration, confirming that the database schema has been updated as required.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_14

LANGUAGE: terminal
CODE:
```
INFO[0000] Auth migrations applied successfully
DEBU[0000] after status
[POP] 2021/12/15 10:44:36 sql - SELECT EXISTS (SELECT schema_migrations.* FROM schema_migrations AS schema_migrations WHERE version = $1) | ["20210710035447"]
[POP] 2021/12/15 10:44:36 sql - SELECT EXISTS (SELECT schema_migrations.* FROM schema_migrations AS schema_migrations WHERE version = $1) | ["20210722035447"]
[POP] 2021/12/15 10:44:36 sql - SELECT EXISTS (SELECT schema_migrations.* FROM schema_migrations AS schema_migrations WHERE version = $1) | ["20210730183235"]
[POP] 2021/12/15 10:44:36 sql - SELECT EXISTS (SELECT schema_migrations.* FROM schema_migrations AS schema_migrations WHERE version = $1) | ["20210909172000"]
[POP] 2021/12/15 10:44:36 sql - SELECT EXISTS (SELECT schema_migrations.* FROM schema_migrations AS schema_migrations WHERE version = $1) | ["20211122151130"]
Version          Name                         Status
20210710035447   alter_users                  Applied
20210722035447   adds_confirmed_at            Applied
20210730183235   add_email_change_confirmed   Applied
20210909172000   create_identities_table      Applied
20211122151130   create_user_id_idx           Applied
```

----------------------------------------

TITLE: Rebuild Supabase Auth Docker Containers
DESCRIPTION: This command fully rebuilds the Supabase Auth Docker containers, ignoring any cached layers. It is particularly useful for ensuring that all dependencies and recent code changes are incorporated into the new container images.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_3

LANGUAGE: bash
CODE:
```
make docker-build
```

----------------------------------------

TITLE: Stop Homebrew PostgreSQL Service to Prevent Port Conflicts
DESCRIPTION: This command is used to stop a locally running PostgreSQL instance managed by Homebrew. It is crucial to execute this if you have an existing PostgreSQL service on port 5432 to prevent port conflicts when running the Supabase Auth database via Docker.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_10

LANGUAGE: shell
CODE:
```
brew services stop postgresql
```

----------------------------------------

TITLE: Install Go 1.22 and Configure PATH on macOS
DESCRIPTION: These commands install Go version 1.22 using Homebrew on macOS and then set the necessary PATH environment variable in the user's .zshrc file. This ensures the Go executable is correctly accessible from the command line.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_4

LANGUAGE: zsh
CODE:
```
brew install go@1.22

echo 'export PATH="/opt/homebrew/opt/go@1.22/bin:$PATH"' >> ~/.zshrc
```

----------------------------------------

TITLE: Configure PostgreSQL Port in Supabase Auth
DESCRIPTION: Instructions for changing the default PostgreSQL port (5432) to a custom port (e.g., 7432) across various configuration files for the Supabase Auth service. This involves updating `docker-compose-dev.yml`, `database.yaml`, `test.env`, and `migrate.sh`.
SOURCE: https://github.com/supabase/auth/blob/master/CONTRIBUTING.md#_snippet_25

LANGUAGE: YAML
CODE:
```
ports:
  - 7432:5432 \ 👈 set the first value to your external facing port
```

LANGUAGE: YAML
CODE:
```
test:
dialect: "postgres"
database: "postgres"
host: {{ envOr "POSTGRES_HOST" "127.0.0.1" }}
port: {{ envOr "POSTGRES_PORT" "7432" }} 👈 set to your port
```

LANGUAGE: Shell
CODE:
```
DATABASE_URL="postgres://supabase_auth_admin:root@localhost:7432/postgres" 👈 set to your port
```

LANGUAGE: Shell
CODE:
```
export GOTRUE_DB_DATABASE_URL="postgres://supabase_auth_admin:root@localhost:7432/$DB_ENV"
```