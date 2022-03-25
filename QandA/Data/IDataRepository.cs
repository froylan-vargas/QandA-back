using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QandA.Data.Models;

namespace QandA.Data
{
    public interface IDataRepository
    {
        IEnumerable<QuestionGetManyResponse> GetQuestions();

        IEnumerable<QuestionGetManyResponse> GetQuestionsBySearch(string search);

        IEnumerable<QuestionGetManyResponse> GetQuestionsBySearchWithPaging(
            string search,
            int pageNumber,
            int pageSize);

        IEnumerable<QuestionGetManyResponse> GetUnansweredQuestions();

        Task<IEnumerable<QuestionGetManyResponse>> GetUnansweredQuestionsAsync();

        Task<QuestionGetSingleResponse> GetQuestion(int questionId);

        Task<bool> QuestionExists(int questionId);

        AnswerGetResponse GetAnswer(int answerId);

        Task<QuestionGetSingleResponse> PostQuestion(QuestionPostFullRequest question);

        Task<QuestionGetSingleResponse> PutQuestion(int questionId, QuestionPutRequest question);

        void DeleteQuestion(int questionId);

        Task<AnswerGetResponse> PostAnswer(AnswerPostFullRequest answer);

        IEnumerable<QuestionGetManyResponse> GetQuestionsWithAnswers();

    }
}
