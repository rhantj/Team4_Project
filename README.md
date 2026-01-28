1. Git Workflow
   - 기능 : feat/(기능 이름)
   - 버그 수정 : fix/(수정된 버그)
   - https://conventional-branch.github.io/

2. Commit
   - 작업 시작 전 main에서 Pull 진행
   - 브랜치 생성 및 작업 진행
   - 코멘트에는 본인의 작업 내용만 간단히 작성
   - main에 직접 푸쉬 후 PR 작성
  
3. Coding Convention
   - 각 단어를 대문자로 사용하는 PascaleCase 사용
   - 멤버 변수 : m_Memmber
     - private float m_FloatNumber
   - Interface : IInterfaceName
     - IMoveable
   - enum : EEnum
     - EState
   - Property : PropertyName
     - public float PropertyOfFloat => Float
