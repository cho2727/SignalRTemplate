사용자 풀 ID : ap-northeast-2_HogtcNwUo
클라이언트 ID : 4e1607j49r7pir03ans8o30hor

[사용자 생성]
aws cognito-idp sign-up ^
  --region ap-northeast-2 ^
  --client-id 4e1607j49r7pir03ans8o30hor ^
  --username test@test.com ^
  --password 12345678
  
[사용자 CONFIRM]  
aws cognito-idp admin-confirm-sign-up ^
  --region ap-northeast-2 ^
  --user-pool-id ap-northeast-2_HogtcNwUo ^
  --username 933e087d-2169-4fc1-ab33-80256cc710fa

[로그인]
aws cognito-idp initiate-auth --region ap-northeast-2 --auth-flow USER_PASSWORD_AUTH --client-id 4e1607j49r7pir03ans8o30hor --auth-parameters USERNAME=test@test.com,PASSWORD=12345678

[토큰확인]
https://jwt.io/

[패키지 설치]
dotnet add package Amazon.AspNetCore.Identity.Cognito
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer