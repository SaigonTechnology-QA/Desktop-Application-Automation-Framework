@echo off
set /p UserInputPath=What Directory would you like?
start cmd /k allure serve %UserInputPath%/allure-results