# Building Builder

자원을 모아 건축물을 완성하는 **3D 캐주얼 건설 시뮬레이션** 게임입니다.

플레이어는 조이스틱으로 맵을 돌아다니며 나무·돌·철을 채집하고, 가공소에 넣어 상위 재료로 만든 뒤,
메인 건축물에 납품해 단계별로 완성시킵니다. 스테이지마다 다른 문명 테마(이집트 / 그리스 / 일본 / 프랑스)의
랜드마크가 목표로 주어지며, 건축이 끝나면 다음 스테이지로 넘어갑니다.

---

## 개발 환경

| 항목 | 값 |
|------|-----|
| 엔진 | Unity **6000.3.5f2** (Unity 6) |
| 렌더 파이프라인 | **URP 17.3.0** |
| 언어 | C# (149개 스크립트) |
| 주요 패키지 | Addressables 2.8.1, AI Navigation 2.0.9, Input System 1.17.0, Timeline |
| 플랫폼 | 모바일 지향 (가상 조이스틱 + 키보드 입력 병행) |
| 시작 씬 | `Assets/06_Scenes/00_MainScene.unity` |

---

## 게임 플레이 루프

```
자원 노드 채집 → 인벤토리에 적재 → 가공소(Production Facility) 투입
   → 상위 재료 생산 → 메인 건축물에 납품 → 단계 완성 → 스테이지 클리어
                    ↘ 판매소(Sales)에 투입 → 골드 획득 → 시설 업그레이드
```

- **채집**: 자원 노드에 접근하면 일정 시간 후 아이템 획득. 노드는 채집 횟수 제한과 리스폰 시간을 가짐
- **인벤토리**: 용량 제한이 있고, 획득한 재료가 캐릭터 뒤에 실제 오브젝트로 쌓임
- **가공소**: 입력 재료를 넣으면 일정 시간 뒤 상위 재료를 출력. 입/출력 슬롯 한도와 업그레이드 존재
- **판매소**: 재료를 골드로 환전
- **건축물**: `SOBuilding`에 정의된 단계별 요구 재료·수량을 채우면 진행도가 오르고, 마지막 단계에서 클리어
- **일꾼(Worker)**: 자동으로 자원을 채집해 수거 구역에 내려놓는 AI. 업그레이드 구역에서 성능 강화

---

## 주요 시스템

### 1. 서비스 로케이터 기반 GameManager

`GameManager`는 로직을 직접 갖지 않고, **런타임에 서비스를 등록/재구성/종료하는 컨테이너** 역할만 합니다.

```csharp
GameManager.Instance.GetService<FacilitySpawner>();
GameManager.Instance.TryGetService<SoundManager>(out var sound);
```

- `IServiceConfig`에 정의된 라이프사이클(`Required`, `RestartAlways`, `Shutdown` 등)에 따라
  서비스가 자동 생성·재설정·파괴됨
- 에디터에서 `OnValidate`가 프로젝트 내 모든 `IServiceConfig` 구현체를 스캔해 인스펙터 목록에 자동 등록
- 등록 서비스: `FacilitySpawner`, `ResourceManager`, `SoundManager`, `GameObjectPoolingService`,
  `PauseService`, `GameObjectTaggedGroupCacheService`

### 2. 스테이지 구성 (데이터 주도)

`SOStageConfig` 하나에 스테이지별 구성이 전부 들어 있습니다.

```
StageStep
 ├─ StageTheme    (테마 맵 프리팹 스폰 요청)
 ├─ MainBuilding  (목표 건축물)
 └─ Resources[]   (자원 노드 / 가공소 / 판매소 배치)
```

`StageManager.BuildStage(idx)`가 이전 스테이지를 정리하고 Addressables로 새 스테이지를
비동기 스폰합니다. 씬 로드가 아니라 **오브젝트 교체 방식**이라 스테이지 전환에 로딩이 끊기지 않습니다.

### 3. 에셋 로딩 / 오브젝트 풀링

- `ResourceManager` — Addressables 프리팹 로드와 캐시. 동일 참조는 재로드하지 않음
- `GameObjectPoolingService` — 스폰 오브젝트 재사용, 정책은 `GameObjectPoolingPolicyModifier`로 지정
- `SpatialHashManager` — 아이템 입출력 구역(`ItemIOArea`)을 셀 단위 해시로 관리해
  전체 순회 없이 주변 3×3 셀만 조회

### 4. 일꾼 AI — Behaviour Tree

`BehaviourTreeSO` + `Blackboard` 조합의 자체 구현 BT입니다.

| 계층 | 노드 |
|------|------|
| Compositor | Sequence, Random, TryInOrder |
| Modifier | Conditional, Cooldown, Inverter, Repeater, InstantRepeater, ResultEnforcer |
| Leaf | 액션 실행 (`BehaviourTreeActionStrategy` 상속 SO) |
| 기타 | Root, NestedTree(트리 중첩) |

일꾼 액션은 SO로 분리되어 있어 코드 수정 없이 트리를 재조립할 수 있습니다 —
`CheckInventory` → `EnrouteToResourceNode` → `WaitToHarvest` → `EnrouteToPickupArea` → `WaitToUnload`.

> `LSY/GOAP/` 아래에 GOAP(A* 플래너) 골격도 있으나 플래너는 미구현 상태의 실험 코드입니다.

### 5. UI — MVVM

UI는 View / ViewModel을 분리해 `IBindable`로 연결합니다.

```
FacilityPanelView  ←  FacilityPanelVM  ←  ProductionFacility
BuildingPanelView  ←  BuildingPanelVM  ←  Building
PickupAreaControlView ← PickupAreaControllerViewModel
```

도메인 객체(`Building`, `ProductionFacility`)는 `event Action`만 발행하고 UI를 모르며,
ViewModel이 이를 구독해 View에 전달합니다. `SimpleBindableProperty`가 값 변경 알림을 담당합니다.

---

## 프로젝트 구조

```
Assets/
├── 01_Scripts/
│   ├── Game Manager/     서비스 로케이터, 서비스 구현체
│   ├── Facility/         가공소(ProductionFacility) + SO
│   ├── Area/             ItemIOArea, SpatialHashManager, 플레이어 감지
│   ├── KMS/              스테이지·건축·시설 스폰, UI(MVVM), 사운드
│   ├── LSY/              일꾼 AI(BT/GOAP), 수거 구역, 카메라 전환, 캐시 서비스
│   ├── OJH/              플레이어 이동·수집·인터랙션, 조이스틱, 자원 노드
│   ├── JJH/              인벤토리, 아이템, 로비·튜토리얼 UI
│   ├── DTO/              ReadOnlyAttribute, SerializableNullable
│   └── Modifiers/        풀링 정책
├── 06_Scenes/            00_MainScene(진입) + 팀원별 작업 씬, Stage01~05
├── 03_Prefabs/           테마별 맵(Egypt/Greece/Japen/France), 건축물, 시설, 아이템 SO
├── 04_Images 05_Animation 08_UI 09_Sounds 10_Font
└── 07_Low Poly, EvSeStudio, Simple Ores... (외부 에셋)
```

---

## 실행 방법

1. Unity Hub에서 **6000.3.5f2** 설치
2. 저장소 클론 후 Unity Hub에 프로젝트 추가
3. `Assets/06_Scenes/00_MainScene.unity` 열고 Play
4. 조작 — 가상 조이스틱 또는 방향키로 이동, 자원/시설 구역에 들어가면 자동 상호작용

> 저장소 용량이 약 240MB입니다.

---

## 팀

4인 팀 프로젝트 (총 173커밋, 브랜치 16개)

| GitHub | 담당 |
|--------|------|
| [@rhantj](https://github.com/rhantj) | 스테이지·자원 매니저, 가공소/건축물, UI 구조(MVVM), 사운드, 테마 맵 |
| [@EnigmaticDoll](https://github.com/EnigmaticDoll) (Seungyoon Lee) | 일꾼 AI(Behaviour Tree), 수거 구역, 카메라 전환, 서비스 구조 |
| [@jun191212](https://github.com/jun191212) (ohjun1999) | 플레이어 이동/수집/인터랙션, 조이스틱, 자원 노드 및 수집 UI |
| [@rorem1](https://github.com/rorem1) | 인벤토리, 아이템 SO, 메인 로비·스테이지 선택·튜토리얼 UI |
