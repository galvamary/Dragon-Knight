# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 프로젝트 개요

드래곤 나이트 - Unity 6 (6000.4.3f1) 기반 2D 횡스크롤 실시간 전투 게임. 학습 목적 프로젝트.

- **게임 컨셉**: 용사가 동료와 함께 마왕을 토벌하는 횡스크롤 액션
- **핵심 시스템**: ATB(Active Time Battle) 기반 스킬 시스템, 동료 소환, 방향 보너스 메커니즘

## 프로젝트 구조

Unity 프로젝트는 루트가 아닌 `DragonKnight/` 서브디렉토리에 위치한다.

```
Dragon-Knight/          ← Git 루트
├── DragonKnight/       ← Unity 프로젝트 루트 (이 폴더를 Unity Hub에서 연다)
│   ├── Assets/         ← 스크립트, 씬, 에셋
│   ├── Packages/       ← 패키지 매니페스트
│   └── ProjectSettings/
└── README.md
```

## 주요 패키지/기술 스택

- **렌더 파이프라인**: URP (Universal Render Pipeline) 17.4.0
- **2D 패키지**: 2D Animation, Aseprite, PSD Importer, SpriteShape, Tilemap + Tilemap Extras
- **입력**: Input System 1.19.0 (New Input System)
- **UI**: uGUI 2.0.0

## .gitignore 주의사항

현재 `.gitignore`가 Git 루트에 있지만 패턴이 Unity 프로젝트 루트 기준(`/[Ll]ibrary/` 등)으로 작성되어 있어, `DragonKnight/Library/`, `DragonKnight/Temp/` 등이 제대로 무시되지 않을 수 있다. `DragonKnight/` 내부에 별도 `.gitignore`를 두거나, 루트 `.gitignore`의 패턴을 `DragonKnight/` 접두사로 수정해야 한다.

## 개발 환경

- Unity 에디터에서 직접 빌드/실행 (CLI 빌드 스크립트 없음)
- C# 스크립트는 `DragonKnight/Assets/` 하위에 작성
- 씬 파일: `DragonKnight/Assets/Scenes/`
