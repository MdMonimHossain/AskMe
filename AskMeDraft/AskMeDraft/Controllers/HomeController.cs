using AskMeDraft.Models;
using AskMeDraft.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AskMeDraft.Controllers
{
    public class HomeController : Controller
    {
        AskMeEntities db = new AskMeEntities();
        public ActionResult Index()
        {
            User user = (User)Session["User"];
            if(user == null)
            {
                User guestUser = new User();
                guestUser.ID = -1;
                guestUser.Role = "guest";
                Session["User"] = guestUser;
            }

            List<Question> questions = (from q in db.Questions
                                        where q.Status == true
                                        orderby q.PostingDatetime descending
                                        select q).ToList();

            List<QuestionCard> questionCards = new List<QuestionCard>();
            foreach (Question question in questions)
            {
                QuestionCard questionCard = new QuestionCard();
                questionCard.ID = question.ID;
                questionCard.CreatorID = question.CreatorID;

                User resultUser = (from u in db.Users
                                   where u.ID == question.CreatorID
                                   select u).FirstOrDefault();

                questionCard.UserName = resultUser.Name;
                questionCard.QuestionTitle = question.Title;
                questionCard.ViewCount = question.ViewCount;
                if (question.PostingDatetime == question.ModificationDatetime)
                {
                    questionCard.Datetime = question.PostingDatetime.ToString("dd MMMM, yyyy hh:mm tt");
                }
                else
                {
                    questionCard.Datetime = question.ModificationDatetime.ToString("dd MMMM, yyyy hh:mm tt") + " (modified)";
                }

                questionCard.AnswerCount = (from a in db.Answers
                                            where a.QuestionID == question.ID
                                            select a).Count();

                questionCards.Add(questionCard);
            }

            ViewBag.QuestionCards = questionCards;

            return View();
        }

        [HttpPost]
        public ActionResult Index(string FilterOption, string SearchKeyword)
        {
            List<Question> questions;

            if (FilterOption == "recent")
            {
                questions = (from q in db.Questions
                             join qt in db.QuestionTags on q.ID equals qt.QuestionID into qt_temp
                             from qt in qt_temp.DefaultIfEmpty()
                             join t in db.Tags on qt.TagID equals t.ID into t_temp
                             from t in t_temp.DefaultIfEmpty()
                             where (q.Title.Contains(SearchKeyword) || t.Keyword.Contains(SearchKeyword)) && q.Status == true
                             orderby q.PostingDatetime descending
                             select q).ToList();
            }
            else if(FilterOption == "mostViewed")
            {
                questions = (from q in db.Questions
                             join qt in db.QuestionTags on q.ID equals qt.QuestionID into qt_temp
                             from qt in qt_temp.DefaultIfEmpty()
                             join t in db.Tags on qt.TagID equals t.ID into t_temp
                             from t in t_temp.DefaultIfEmpty()
                             where (q.Title.Contains(SearchKeyword) || t.Keyword.Contains(SearchKeyword)) && q.Status == true
                             orderby q.ViewCount descending
                             select q).ToList();
            }
            else
            {
                questions = (from q in db.Questions
                             join qt in db.QuestionTags on q.ID equals qt.QuestionID into qt_temp
                             from qt in qt_temp.DefaultIfEmpty()
                             join t in db.Tags on qt.TagID equals t.ID into t_temp
                             from t in t_temp.DefaultIfEmpty()
                             where (q.Title.Contains(SearchKeyword) || t.Keyword.Contains(SearchKeyword)) && q.Status == true
                             orderby q.PostingDatetime descending
                             select q).ToList();
                
                foreach (Question question in questions.ToList())
                {
                    if ((from a in db.Answers
                         where a.QuestionID == question.ID
                         select a).Count() != 0)
                    {
                        questions.Remove(question);
                    }
                }
            }

            questions = questions.GroupBy(x => x.ID).Select(y => y.First()).ToList();

            List<QuestionCard> questionCards = new List<QuestionCard>();
            foreach (Question question in questions)
            {
                QuestionCard questionCard = new QuestionCard();
                questionCard.ID = question.ID;
                questionCard.CreatorID = question.CreatorID;

                User resultUser = (from u in db.Users
                                   where u.ID == question.CreatorID
                                   select u).FirstOrDefault();

                questionCard.UserName = resultUser.Name;
                questionCard.QuestionTitle = question.Title;
                questionCard.ViewCount = question.ViewCount;
                if (question.PostingDatetime == question.ModificationDatetime)
                {
                    questionCard.Datetime = question.PostingDatetime.ToString("dd MMMM, yyyy hh:mm tt");
                }
                else
                {
                    questionCard.Datetime = question.ModificationDatetime.ToString("dd MMMM, yyyy hh:mm tt") + " (modified)";
                }

                questionCard.AnswerCount = (from a in db.Answers
                                            where a.QuestionID == question.ID
                                            select a).Count();

                questionCards.Add(questionCard);
            }

            ViewBag.QuestionCards = questionCards;
            ViewBag.FilterOption = FilterOption;
            ViewBag.SearchKeyword = SearchKeyword;

            return View();
        }


        public ActionResult UserProfile(int? ID)
        {
            User user = (User)Session["User"];
            if (user != null)
            {
                if (ID == null)
                {
                    ViewBag.User = user;
                }
                else
                {
                    User resultUser = (from u in db.Users
                                       where u.ID == ID
                                       select u).SingleOrDefault();
                    ViewBag.User = resultUser;
                }
                return View();
            }
            else
                return RedirectToAction("Login", "User");
        }

        public ActionResult MyQuestions()
        {
            User user = (User)Session["User"];
            if (user != null)
            {
                List<Question> questions = (from q in db.Questions
                                            where q.CreatorID == user.ID
                                            select q).ToList();

                List<QuestionCard> questionCards = new List<QuestionCard>();
                foreach (Question question in questions)
                {
                    QuestionCard questionCard = new QuestionCard();
                    questionCard.ID = question.ID;
                    questionCard.CreatorID = question.CreatorID;
                    questionCard.UserName = user.Name;
                    questionCard.QuestionTitle = question.Title;
                    questionCard.ViewCount = question.ViewCount;
                    if(question.PostingDatetime == question.ModificationDatetime)
                    {
                        questionCard.Datetime = question.PostingDatetime.ToString("dd MMMM, yyyy hh:mm tt");
                    }
                    else
                    {
                        questionCard.Datetime = question.ModificationDatetime.ToString("dd MMMM, yyyy hh:mm tt") + " (modified)";
                    }
                    
                    questionCard.AnswerCount = (from a in db.Answers
                                                where a.QuestionID == question.ID
                                                select a).Count();

                    questionCards.Add(questionCard);
                }

                ViewBag.QuestionCards = questionCards;

                return View();
            }
            else
                return RedirectToAction("Login", "User");
        }

        public ActionResult MyAnswers()
        {
            User sessionUser = (User)Session["User"];
            if (sessionUser != null)
            {
                List<Answer> answers = (from a in db.Answers
                                        where a.CreatorID == sessionUser.ID
                                        select a).ToList();
                List<AnswerCard> answerCards = new List<AnswerCard>();

                foreach (Answer answer in answers)
                {
                    AnswerCard answerCard = new AnswerCard();
                    answerCard.ID = answer.ID;
                    answerCard.QuestionID = answer.QuestionID;
                    answerCard.Content = answer.Content;
                    answerCard.UpvoteCount = answer.UpvoteCount;
                    answerCard.DownvoteCount = answer.DownvoteCount;
                    answerCard.Status = answer.Status;

                    if (answer.PostingDatetime == answer.ModificationDatetime)
                    {
                        answerCard.Datetime = answer.PostingDatetime.ToString("dd MMMM, yyyy hh:mm tt");
                    }
                    else
                    {
                        answerCard.Datetime = answer.ModificationDatetime.ToString("dd MMMM, yyyy hh:mm tt") + " (modified)";
                    }

                    User user = (from u in db.Users
                                 where u.ID == answer.CreatorID
                                 select u).SingleOrDefault();

                    answerCard.CreatorID = user.ID;
                    answerCard.UserName = user.Name;

                    answerCards.Add(answerCard);
                }

                ViewBag.AnswerCards = answerCards;

                return View();
            }
            else
                return RedirectToAction("Login", "User");
        }

        public ActionResult Reports()
        {
            User user = (User)Session["User"];
            if (user != null)
            {
                List<QuestionReport> questionReports = (from qr in db.QuestionReports
                                                        orderby qr.Status descending
                                                        select qr).ToList();

                List<QuestionReportView> questionReportViews = new List<QuestionReportView>();
                
                foreach(QuestionReport questionReport in questionReports)
                {
                    QuestionReportView questionReportView = new QuestionReportView();
                    questionReportView.ID = questionReport.ID;
                    questionReportView.QuestionID = questionReport.QuestionID;
                    questionReportView.Reason = questionReport.Reason;
                    questionReportView.ReportDatetime = questionReport.ReportDatetime.ToString("dd MMMM, yyyy hh:mm tt");
                    questionReportView.Status = questionReport.Status;
                    questionReportView.ReporterID = questionReport.ReporterID;
                    questionReportView.ReporterName = (from u in db.Users
                                                       where u.ID == questionReport.ReporterID
                                                       select u).SingleOrDefault().Name;
                    questionReportView.ReportHandlerID = questionReport.ReportHandlerID;
                    if (questionReport.ReportHandlerID != null)
                    {
                        questionReportView.ReportHandlerName = (from u in db.Users
                                                                where u.ID == questionReport.ReportHandlerID
                                                                select u).SingleOrDefault().Name;
                    }
                    else
                    {
                        questionReportView.ReportHandlerName = "";
                    }
                    questionReportViews.Add(questionReportView);
                }
                
                ViewBag.QuestionReportViews = questionReportViews;

                List<AnswerReport> answerReports = (from ar in db.AnswerReports
                                                    orderby ar.Status descending
                                                    select ar).ToList();

                List<AnswerReportView> answerReportViews = new List<AnswerReportView>();

                foreach (AnswerReport answerReport in answerReports)
                {
                    AnswerReportView answerReportView = new AnswerReportView();
                    answerReportView.ID = answerReport.ID;
                    answerReportView.AnswerID = answerReport.AnswerID;
                    answerReportView.Reason = answerReport.Reason;
                    answerReportView.ReportDatetime = answerReport.ReportDatetime.ToString("dd MMMM, yyyy hh:mm tt");
                    answerReportView.Status = answerReport.Status;
                    answerReportView.ReporterID = answerReport.ReporterID;
                    answerReportView.ReporterName = (from u in db.Users
                                                     where u.ID == answerReport.ReporterID
                                                     select u).SingleOrDefault().Name;
                    answerReportView.ReportHandlerID = answerReport.ReportHandlerID;
                    if (answerReport.ReportHandlerID != null)
                    {
                        answerReportView.ReportHandlerName = (from u in db.Users
                                                              where u.ID == answerReport.ReportHandlerID
                                                              select u).SingleOrDefault().Name;
                    }
                    else
                    {
                        answerReportView.ReportHandlerName = "";
                    }
                    answerReportViews.Add(answerReportView);
                }

                ViewBag.AnswerReportViews = answerReportViews;

                return View();
            }
            else
                return RedirectToAction("Login", "User");
        }

        public ActionResult Banned()
        {
            User user = (User)Session["User"];
            if (user != null)
            {
                List<Question> questions = (from q in db.Questions
                                            where q.Status == false
                                            select q).ToList();
                List<QuestionCard> questionCards = new List<QuestionCard>();
                foreach (Question question in questions)
                {
                    QuestionCard questionCard = new QuestionCard();
                    questionCard.ID = question.ID;
                    questionCard.CreatorID = question.CreatorID;

                    User resultUser = (from u in db.Users
                                       where u.ID == question.CreatorID
                                       select u).FirstOrDefault();

                    questionCard.UserName = resultUser.Name;
                    questionCard.QuestionTitle = question.Title;
                    questionCard.ViewCount = question.ViewCount;
                    if (question.PostingDatetime == question.ModificationDatetime)
                    {
                        questionCard.Datetime = question.PostingDatetime.ToString("dd MMMM, yyyy hh:mm tt");
                    }
                    else
                    {
                        questionCard.Datetime = question.ModificationDatetime.ToString("dd MMMM, yyyy hh:mm tt") + " (modified)";
                    }
                    questionCard.AnswerCount = (from a in db.Answers
                                                where a.QuestionID == question.ID
                                                select a).Count();

                    questionCards.Add(questionCard);
                }

                ViewBag.QuestionCards = questionCards;

                List<Answer> answers = (from a in db.Answers
                                        where a.Status == false
                                        select a).ToList();
                List<AnswerCard> answerCards = new List<AnswerCard>();

                foreach (Answer answer in answers)
                {
                    AnswerCard answerCard = new AnswerCard();
                    answerCard.ID = answer.ID;
                    answerCard.QuestionID = answer.QuestionID;
                    answerCard.Content = answer.Content;
                    answerCard.UpvoteCount = answer.UpvoteCount;
                    answerCard.DownvoteCount = answer.DownvoteCount;
                    answerCard.Status = answer.Status;

                    if (answer.PostingDatetime == answer.ModificationDatetime)
                    {
                        answerCard.Datetime = answer.PostingDatetime.ToString("dd MMMM, yyyy hh:mm tt");
                    }
                    else
                    {
                        answerCard.Datetime = answer.ModificationDatetime.ToString("dd MMMM, yyyy hh:mm tt") + " (modified)";
                    }

                    User resultUser = (from u in db.Users
                                       where u.ID == answer.CreatorID
                                       select u).SingleOrDefault();

                    answerCard.CreatorID = resultUser.ID;
                    answerCard.UserName = resultUser.Name;

                    answerCards.Add(answerCard);
                }

                ViewBag.AnswerCards = answerCards;

                List<User> bannedUsers = (from u in db.Users
                                          where u.Status == false
                                          select u).ToList();

                ViewBag.BannedUsers = bannedUsers;

                return View();
            }
            else
                return RedirectToAction("Login", "User");
        }
            
    }
}