# AprsWeather

Weather maps backed by data from the Automatic Position Reporting System (APRS).

## Running Locally

Before running, you'll have to set a value for the `APRS_IS_CALLSIGN` environment variable.
This value will be used for APRS-IS login.

The full application can be run locally using `docker compose up -d --build`.
Once that is complete, load `http://localhost:80` in your browser or use `http://localhost:5148/WeatherReports` to test the backend.

Once finished, it can be shut down with `docker compose down`.
