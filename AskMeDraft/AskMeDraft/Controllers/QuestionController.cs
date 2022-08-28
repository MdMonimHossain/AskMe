using AskMeDraft.Models;
using AskMeDraft.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AskMeDraft.Controllers
{
    public class QuestionController : Controller
    {
        AskMeEntities db = new AskMeEntities();
        
        public ActionResult PostQuestion()
        {
            User user = (User)Session["User"];
            if (user != null)
                return View();
            else
                return RedirectToAction("Login", "User");
        }

        [HttpPost]
        public ActionResult PostQuestion(PostQuestion postQuestion)
        {
            User user = (User)Session["User"];
            
            if(ModelState.IsValid)
            {
                //add question
                Question question = new Question();
                question.Title = postQuestion.Title;
                question.Description = postQuestion.Description;
                question.CreatorID = user.ID;
                question.PostingDatetime = DateTime.Now;
                question.ModificationDatetime = DateTime.Now;
                question.ViewCount = 0;
                question.Status = true;
                db.Questions.Add(question);
                db.SaveChanges();

                //add tags
                if(postQuestion.Tags != null)
                {
                    List<string> tags = postQuestion.Tags.Split(' ').ToList();

                    foreach (string tag in tags)
                    {
                        if(!string.IsNullOrWhiteSpace(tag))
                        {
                            Tag newTag = null;
                            newTag = (from t in db.Tags
                                      where t.Keyword == tag
                                      select t).FirstOrDefault();

                            if (newTag == null)
                            {
                                newTag = new Tag();
                                newTag.Keyword = tag;
                                db.Tags.Add(newTag);
                                db.SaveChanges();
                            }

                            QuestionTag questionTag = new QuestionTag();
                            questionTag.QuestionID = question.ID;
                            questionTag.TagID = newTag.ID;
                            db.QuestionTags.Add(questionTag);
                            db.SaveChanges();
                        }
                    }
                }
                
                //increase question count of user
                User resultUser = (from u in db.Users
                               where u.ID == user.ID
                               select u).SingleOrDefault();
                resultUser.QuestionCount++;
                db.SaveChanges();

                Session["User"] = resultUser;

                return RedirectToAction("MyQuestions", "Home");
            }

            ViewBag.QuestionTitle = postQuestion.Title;
            ViewBag.Description = postQuestion.Description;

            return View();
        }

        public ActionResult EditQuestion(int? ID)
        {
            if (ID == null)
                return View("Error");

            User user = (User)Session["User"];
            if (user != null)
            {
                Question question = (from q in db.Questions
                                     where q.ID == ID
                                     select q).SingleOrDefault();

                ViewBag.QuestionID = question.ID;
                ViewBag.QuestionTitle = question.Title;
                ViewBag.Description = question.Description;

                List<Tag> tags = (from t in db.Tags
                                  join qt in db.QuestionTags on t.ID equals qt.TagID
                                  where qt.QuestionID == question.ID
                                  select t).ToList();

                string tagString = "";
                foreach (Tag tag in tags)
                {
                    tagString += tag.Keyword + " ";
                }

                ViewBag.Tags = tagString;

                return View();
            }
            else
                return RedirectToAction("Login", "User");
        }

        [HttpPost]
        public ActionResult EditQuestion(EditQuestion editQuestion)
        {
            User user = (User)Session["User"];
            if (user != null)
            {
                if(ModelState.IsValid)
                {
                    Question question = (from q in db.Questions
                                         where q.ID == editQuestion.ID
                                         select q).SingleOrDefault();

                    question.Title = editQuestion.Title;
                    question.Description = editQuestion.Description;
                    question.ModificationDatetime = DateTime.Now;
                    db.SaveChanges();

                    //delete all tags
                    List<QuestionTag> questionTags = (from qt in db.QuestionTags
                                                      where qt.QuestionID == question.ID
                                                      select qt).ToList();
                    foreach (QuestionTag questionTag in questionTags)
                    {
                        db.QuestionTags.Remove(questionTag);
                        db.SaveChanges();
                    }

                    //add tags
                    if (editQuestion.Tags != null)
                    {
                        List<string> tags = editQuestion.Tags.Split(' ').ToList();

                        foreach (string tag in tags)
                        {
                            if (!string.IsNullOrWhiteSpace(tag))
                            {
                                Tag newTag = null;
                                newTag = (from t in db.Tags
                                          where t.Keyword == tag
                                          select t).FirstOrDefault();

                                if (newTag == null)
                                {
                                    newTag = new Tag();
                                    newTag.Keyword = tag;
                                    db.Tags.Add(newTag);
                                    db.SaveChanges();
                                }

                                QuestionTag questionTag = new QuestionTag();
                                questionTag.QuestionID = question.ID;
                                questionTag.TagID = newTag.ID;
                                db.QuestionTags.Add(questionTag);
                                db.SaveChanges();
                            }
                        }
                    }

                    return RedirectToAction("MyQuestions", "Home");
                }

                ViewBag.QuestionID = editQuestion.ID;
                ViewBag.QuestionTitle = editQuestion.Title;
                ViewBag.Description = editQuestion.Description;
                ViewBag.Tags = editQuestion.Tags;

                return View();
            }
            else
                return RedirectToAction("Login", "User");
        }

        public ActionResult DeleteQuestion(int? ID)
        {
            if (ID == null)
                return View("Error");

            User user = (User)Session["User"];
            if (user != null)
            {
                Question question = (from q in db.Questions
                                     where q.ID == ID
                                     select q).SingleOrDefault();
                
                //decrease question count of user
                User resultUser = (from u in db.Users
                             where u.ID == question.CreatorID
                             select u).SingleOrDefault();

                resultUser.QuestionCount--;
                db.Questions.Remove(question);
                db.SaveChanges();

                Session["User"] = resultUser;

                return RedirectToAction("MyQuestions", "Home");
            }
            else
                return RedirectToAction("Login", "User");
        }
        
        public ActionResult ViewQuestion(int? ID, bool increaseViews = false)
        {
            if (ID == null)
                return View("Error");

            Question question = (from q in db.Questions
                                 where q.ID == ID
                                 select q).SingleOrDefault();

            User resultUser = (from u in db.Users
                               where u.ID == question.CreatorID
                               select u).SingleOrDefault();

            User sessionUser = (User)Session["User"];
            if (sessionUser.ID != resultUser.ID && increaseViews)
            {
                question.ViewCount++;
                db.SaveChanges();
            }
            
            ViewBag.Question = question;

            if (question.PostingDatetime == question.ModificationDatetime)
            {
                ViewBag.Datetime = question.PostingDatetime.ToString("dd MMMM, yyyy hh:mm tt");
            }
            else
            {
                ViewBag.Datetime = question.ModificationDatetime.ToString("dd MMMM, yyyy hh:mm tt") + " (modified)";
            }

            ViewBag.User = resultUser;
            ViewBag.Tags = (from t in db.Tags
                            join qt in db.QuestionTags on t.ID equals qt.TagID
                            where qt.QuestionID == question.ID
                            select t).ToList();

            List<Answer> answers = (from a in db.Answers
                                    where a.QuestionID == question.ID && a.Status == true
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
            ViewBag.hideViewQuestion = "Display: none;";

            return View();
        }

        public ActionResult ReportQuestion(int? ID)
        {
            if (ID == null)
                return View("Error");

            ViewBag.QuestionID = ID;
            return View();
        }

        [HttpPost]
        public ActionResult ReportQuestion(int QuestionID, string Reason)
        {
            User sessionUser = (User)Session["User"];

            QuestionReport questionReport = new QuestionReport
            {
                QuestionID = QuestionID,
                ReporterID = sessionUser.ID,
                Reason = Reason,
                ReportDatetime = DateTime.Now,
                Status = true
            };
            
            db.QuestionReports.Add(questionReport);

            try
            {
                db.SaveChanges();

                Question question = (from q in db.Questions
                                     where q.ID == QuestionID
                                     select q).SingleOrDefault();
                User resultUser = (from u in db.Users
                                   where u.ID == question.CreatorID
                                   select u).SingleOrDefault();
                resultUser.ReportCount++;
                db.SaveChanges();
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "You have already reported this question");
                ViewBag.QuestionID = QuestionID;
                return View();
            }

            return RedirectToAction("ViewQuestion", "Question", new { ID = QuestionID });
        }
        
        public ActionResult ChangeQuestionStatus(int? ID)
        {
            if (ID == null)
                return View("Error");

            Question question = (from q in db.Questions
                                 where q.ID == ID
                                 select q).SingleOrDefault();
            question.Status = !question.Status;
            db.SaveChanges();
            return RedirectToAction("ViewQuestion", "Question", new { ID = question.ID });
        }

        [HttpPost]
        public JsonResult ChangeReportStatus(int QuestionReportID)
        {
            User sessionUser = (User)Session["User"];

            QuestionReport questionReport = (from qr in db.QuestionReports
                                             where qr.ID == QuestionReportID
                                             select qr).SingleOrDefault();
            if (questionReport.Status)
            {
                questionReport.ReportHandlerID = sessionUser.ID;
            }
            else
            {
                questionReport.ReportHandlerID = null;
            }
            questionReport.Status = !questionReport.Status;
            db.SaveChanges();

            if (questionReport.Status)
            {
                return Json(new { ReportStatus = "Pending", ReportHandlerID = -1, ReportHandlerName = "" });
            }
            else
            {
                User reportHandler = (from u in db.Users
                                      where u.ID == questionReport.ReportHandlerID
                                      select u).SingleOrDefault();

                return Json(new { ReportStatus = "Resolved", ReportHandlerID = reportHandler.ID, ReportHandlerName = reportHandler.Name });
            }

        }
    }
}