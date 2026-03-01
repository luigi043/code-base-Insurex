# Criar usuário admin via API
curl.exe -X POST http://localhost:5012/api/v1/Auth/register `
  -H "Content-Type: application/json" `
  -d '{\"name\":\"Admin\",\"email\":\"admin@insurex.com\",\"password\":\"admin123\"}'