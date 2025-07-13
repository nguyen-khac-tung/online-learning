# Quiz Feature Implementation

## Tổng quan
Chức năng Quiz cho phép học viên làm bài kiểm tra sau khi hoàn thành các module. Mỗi module có thể có nhiều quiz với các câu hỏi và đáp án khác nhau.

## Cấu trúc Database

### Entities chính:
- **Quiz**: Thông tin quiz (tên, thời gian, điểm đạt, số câu hỏi)
- **Question**: Câu hỏi trong quiz
- **Option**: Các lựa chọn cho câu hỏi (có 1 đáp án đúng)
- **UserQuizResult**: Kết quả quiz của user
- **UserAnswer**: Câu trả lời của user cho từng câu hỏi

### Relationships:
- Quiz (1) -> (N) Question
- Question (1) -> (N) Option
- User (1) -> (N) UserQuizResult
- UserQuizResult (1) -> (N) UserAnswer

## API Endpoints

### 1. Lấy thông tin Quiz
```
GET /api/quiz/{quizId}
```
Trả về thông tin quiz với danh sách câu hỏi và đáp án (không bao gồm đáp án đúng)

### 2. Lấy danh sách Quiz theo Module
```
GET /api/quiz/module/{moduleId}
```
Trả về danh sách quiz của một module

### 3. Nộp bài Quiz
```
POST /api/quiz/submit
```
Body:
```json
{
  "quizId": 1,
  "answers": [
    {
      "questionId": 1,
      "optionId": 1
    },
    {
      "questionId": 2,
      "optionId": 5
    }
  ]
}
```

### 4. Xem kết quả Quiz
```
GET /api/quiz/result/{quizId}
```
Trả về kết quả chi tiết với đáp án đúng và sai

### 5. Lấy danh sách kết quả Quiz của User
```
GET /api/quiz/results
```

### 6. Kiểm tra User đã hoàn thành Quiz chưa
```
GET /api/quiz/completed/{quizId}
```

## Business Logic

### Tính điểm:
- Điểm = (Số câu đúng / Tổng số câu) * 100
- Pass nếu điểm >= PassScore hoặc PassScore = null

### Validation:
- User chỉ được làm quiz 1 lần
- Số câu trả lời phải bằng số câu hỏi
- Quiz phải tồn tại và active

### Tính năng:
- Lưu thời gian bắt đầu và kết thúc
- Tính thời gian làm bài
- Lưu chi tiết từng câu trả lời
- Hiển thị đáp án đúng và sai

## Cấu trúc Code

### DTOs:
- `QuizRequestDto`: Request khi submit quiz
- `QuizResponseDTO`: Response khi lấy thông tin quiz
- `QuizResultResponseDTO`: Response khi xem kết quả

### Repositories:
- `IQuizRepository`: CRUD cho Quiz
- `IQuestionRepository`: CRUD cho Question
- `IUserQuizResultRepository`: CRUD cho UserQuizResult
- `IUserAnswerRepository`: CRUD cho UserAnswer

### Services:
- `IQuizService`: Business logic cho quiz

### Controllers:
- `QuizController`: API endpoints

## Cách sử dụng

1. **Tạo Quiz**: Admin tạo quiz với questions và options
2. **User làm Quiz**: User truy cập quiz và trả lời câu hỏi
3. **Submit**: User nộp bài và nhận kết quả ngay lập tức
4. **Xem kết quả**: User có thể xem lại kết quả chi tiết

## Lưu ý

- Tất cả API đều yêu cầu authentication
- User chỉ được làm quiz 1 lần duy nhất
- Điểm được tính tự động dựa trên số câu đúng
- Hệ thống lưu đầy đủ thông tin để có thể review sau này 