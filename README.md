# AprsWeather

Weather maps backed by data from the Automatic Position Reporting System (APRS).

## Running Locally

Before running, you'll have to set a value for the `APRS_IS_CALLSIGN` environment variable.
This value will be used for APRS-IS login.

The full application can be run locally using `docker compose up -d --build`.
Once that is complete, load `http://localhost:80` in your browser or use `http://localhost:5148/WeatherReports` to test the backend.

Once finished, it can be shut down with `docker compose down`.

## Deploying

This application deploys with [DigitalOcean App Platform](https://www.digitalocean.com/products/app-platform).
It has full continuous deployment for any code pushed/merged to `main`.

To bootstrap the deployment, use the [doctl](https://docs.digitalocean.com/reference/doctl/) command line to deploy the application.
Once deployed, take the application ID and save it as a GitHub Actions secret so it can be used for future updates and deployments.

## APRS Reports and Gridsquare Locations

Good resources for finding and viewing APRS reports and gridsquare locations (good for debugging):

* [Google Maps APRS (APRS.fi)](https://aprs.fi)
* [Amateur Radio Ham Radio Maidenhead Grid Square Locator Map](https://www.levinecentral.com/ham/grid_square.php)
* [GridMapper by QRZ Ham Radio](https://www.qrz.com/gridmapper)
