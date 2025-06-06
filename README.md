# Gradius 🔫

FSM 기반 공격 패턴, 풀링 시스템, Profiler를 통한 성능 분석 및 구조 개선을 중점으로 진행한 개인 프로젝트입니다.

## 📌 프로젝트 개요

- 장르: 2D 횡스크롤 슈팅 (모작 + 창작)
- 기간: 2022.1 + 리팩토링 (2022.11~2023.02)
- 역할: 전체 개발

## 🔧 주요 기능

- FSM 패턴 기반 적 공격 구조
- Object Pooling + Poolable 인터페이스 구조
- Draw Call 최적화를 위한 UI 구조
- Profiler 분석 → 리팩토링 적용

## 💡 기술 포인트

- Queue 기반 풀링 개선
- Pool 초과 시 생성 제한 + 재사용 로직

## SideScrolling_Gradius : Multi Play
The new version that has Single Play and Multi Play  
Using the API called "Nakama"  
Projects for understanding the work process between game servers and clients  

## Getting Started
You will need to install Docker Desktop (or need Docker and docker-compose)  
You will find a docker-compose.yml file in the directory of the project  
Open a terminal (like CMD) and type : docker-compose up (or docker compose up)  
Open a Unity Project and Build to Windows game, and play!!  

## WARNING!!!
This game is a simple personal project  
So please do not use anything included in the game for commercial purposes  
