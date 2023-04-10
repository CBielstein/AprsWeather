#!/bin/sh
if [ $1 == "Production" ];
then
    sed -i 's|<!--INSERT: Plausible Analytics scripts-->|<script defer data-domain="hamwx.bielstein.dev" src="https://plausible.io/js/script.js"></script>|' AprsWeatherClient/wwwroot/index.html
fi
