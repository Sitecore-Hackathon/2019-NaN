$env:xconnect="d:\\src\\SC91\Runtime\\websites\\sc910.xconnect.local"

Copy-Item -Path "ContactModel, 1.0.json" -Destination "$Env:xconnect\\App_Data\\jobs\\continuous\\ProcessingEngine\\App_Data\\Models" -Force
Copy-Item -Path "ContactModel, 1.0.json" -Destination "$Env:xconnect\\App_Data\\Models" -Force
Copy-Item -Path "ContactModel, 1.0.json" -Destination "$Env:xconnect\\App_Data\\jobs\\continuous\\IndexWorker\\App_data\\Models" -Force

Write-Output "Success!";