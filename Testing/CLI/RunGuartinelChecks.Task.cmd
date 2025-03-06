@echo off

:repeat_again

call RunGuartinelChecks.cmd

timeout /T 300 /nobreak

goto repeat_again
