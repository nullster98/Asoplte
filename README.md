markdown
# Project Apostle ⚔️

**선택에 따라 운명이 바뀌는 2D 로그라이크 게임**

[![Status](https://img.shields.io/badge/status-in_development-yellow)](https://github.com/nyangnyangAI/Apostle)

## ✨ 프로젝트 개요

Project Apostle은 선택지 기반 2D 싱글 로그라이크 게임이다냥! 플레이어의 선택과 컨셉이 게임의 흐름에 실질적인 영향을 주도록 설계되었다냥!

*   🎮 장르: 2D 로그라이크
*   👤 개발: 1인 개발
*   🛠️ 엔진: Unity
*   📜 언어: C#

## 🎯 개발 목표

플레이어의 선택과 컨셉이 게임의 흐름에 **실질적** 영향을 주는 구조 구현 🚀

## 🗓️ 개발 기간

*   2024년 하반기 (약 8일): 컨셉 구상 및 초안 작업
*   2025년 3월 3일 ~ 현재: 개발 진행 중 ⏳

## ⚙️ 개발 환경 및 역할

*   Unity, C# 활용
*   1인 개발 프로젝트

## 🧩 주요 기능

### 📜 기획 파트

*   **기획 의도**: 전략 턴제 로그라이크의 틀에서 벗어난 새로운 게임 개발 🔥
*   **기획 컨셉 및 개발 배경**: 1인 개발 도전과 선택 기반 성장형 게임
*   **핵심 재미요소**:
    1.  **전투는 선택**: 전투를 회피하거나 필연적으로 마주치는 전략적 선택 🛡️
    2.  **컨셉 플레이의 몰입감**: 신앙/종족/특성이 플레이에 직접적인 영향 🎭
    3.  **선택의 축적과 파급력**: 선택이 게임 전체에 영향을 주는 경험 🔄
*   **세계관 및 배경 설정**:
    *   잊혀진 고대의 신들, 던전에 잠들다.
    *   마지막 남은 신앙으로 사도를 창조!
    *   사도를 통해 신앙을 전파하고 던전을 탐험하는 여정.
    *   "당신은 신이다. 사도를 보내 세상을 다시 기억시켜라." 🌟
*   **주요 테마**:
    *   신과 사도: 플레이어는 신의 입장에서 사도를 인도 👼
    *   신앙의 회복: 잊혀진 신들이 신앙심을 되찾기 위한 노력 🙏
    *   선택의 무게: 작은 결정이 세계를 바꾼다 ⚖️
    *   매번 다른 여정: 신앙, 종족, 특성 조합에 따라 변화하는 이야기 🗺️

### 💻 개발 파트

*   **Data 중심 설계**: `DatabaseManager` 시스템으로 통합 관리 🗄️
*   **효과 구조 통합**: `IEffect` 인터페이스로 신/종족/특성 통합 🔗
*   **대상 통합 처리**: `IUnit` 인터페이스로 모든 유닛 기능 정의 🎯
*   **효과 처리 자동화**: `EffectFactory & Registry` 시스템으로 효과 처리 ⚙️
*   **데이터 자동화**: Google Sheet + Json 파이프라인으로 데이터 관리 📊
    *   [Google Sheet Link]([https://docs.google.com/spreadsheets/d/1aq027V04s4wilLNBEvcgEEYGdaq6YAwCn2dw_Szd2rE/edit?usp=sharing](https://docs.google.com/spreadsheets/d/1aq027VO4s4wilLNBEvcgEEYGdaq6YAwCn2dw_Szd2rE/edit?gid=0#gid=0)) 🌐
*   **선택 및 분기처리**: 이벤트 구조 시스템으로 분기 로직 관리 🔀
*   **동적 생성**: Entity 구조 시스템으로 객체 동적 생성 ➕

**다이어그램: 데이터 자동화**

GoogleSheet 작성 → CSV파일로  저장 → Json 변환 → JsonLoader → JsonHelper → XXXDataLine → DatabaseManager LoadAll() 함수 📈

**다이어그램: 이벤트 구조**

분기형 이벤트 구조: Event(Main) > Phase > DialogueBlock > Choice

### 📜 (OutCome & Condition) + DSL 명령어

*   해당 클래스 내에서 현재 신/종족/특성/신앙심 수치를 체크하는 로직 담당

*   명령어를 Google Sheet에 형식에 맞게 기입 시 해당 조건을 `OutcomParser`에서 체크 후 분리, 적용
*   `Condition` 또한 `Parser`에서 체크 후 분리, 적용

DSL 명령어 예시:

*   `Stat:Atk+10`: 스탯 증가
*   `Trait:add_TP01`: 특성 추가
*   `Reward:Item:1004`: 보상 아이템 지급

### 🎮 게임 시스템

*   주요 선택 요소
    *   신앙
    *   종족
    *   특성
*   유저 경험 예상 흐름도

    타이틀 화면 → 신앙 선택 → 사도 탄생 (종족/특성 선택) → 1층 이벤트 시작 → 이벤트 진행 → 선택지/결과 분기 → LOOP → 보상/패널티 → Entity와의 상호작용 → 엔딩 화면 → 엔딩 분기 → 10층: 보스 → 9층: 휴식 🕹️

## 💡 기획 이유

*   **Baldur's Gate**: 선택의 자유, 분기된 결과, 역할극 몰입 🎭
*   **Tower of Winter**: 텍스트 기반 로그라이크, 간결한 루프, 중독성 🔁
*   사악한 기운... (냥냥이는 더 이상 말할 수 없다냥!) 🤫

## 🚀 향후 계획

*   다양한 신앙, 종족, 특성 추가 ➕
*   스토리 및 이벤트 다양화 📚
*   UI/UX 개선 🎨
*   최적화 및 버그 수정 🐛

## 🤝 기여 방법

1.  Fork this repository!
2.  Create your feature branch: `git checkout -b my-new-feature`
3.  Commit your changes: `git commit -am 'Add some feature'`
4.  Push to the branch: `git push origin my-new-feature`
5.  Submit a pull request!

## 📄 라이선스

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.
