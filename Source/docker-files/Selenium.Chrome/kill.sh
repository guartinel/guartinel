#!/bin/bash
echo Sleeping before killing...
sleep $[ ( $RANDOM % 10 )  + 1 ]m
echo Killing..
kill 1