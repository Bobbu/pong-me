# Spotify Integration Research

**Date:** 2026-04-04
**Status:** Parked — revisit after core game is solid

## Key Findings

- No native audio streaming SDK for C#/Unity — only SpotifyAPI-NET for playback control of external Spotify app
- Users need Spotify Premium for playback control
- Dev mode limited to 5 test users; wider distribution needs extended quota application
- Spotify policy prohibits music-tied gameplay (rhythm games, quizzes)
- Viable approach: "now playing" HUD + play/pause/skip controls, with Spotify running separately

## Why Parked

User wants background music/playlist support but constraints are too heavy for initial development phase.

## When We Revisit

- Start with SpotifyAPI-NET package and OAuth2 Authorization Code Flow
- Focus on cosmetic integration only (HUD + controls), not audio streaming
- Plan for extended quota application if distributing beyond 5 test users
