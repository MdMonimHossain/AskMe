using AskMeDraft.Models;
using AskMeDraft.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AskMeDraft.Controllers
{
    public class AnswerController : Controller
    {
        AskMeEntities db = new AskMeEntities();
        
        [HttpPost]
        public ActionResult PostAnswer(PostAnswer postAnswer)
        {
            User user = (User)Session["User"];
            if (user != null)
            {
                if (ModelState.IsValid)
                {
                    //add answer
                    Answer answer = new Answer();
                    answer.Content = postAnswer.Content;
                    answer.CreatorID = user.ID;
                    answer.QuestionID = postAnswer.QuestionID;
                    answer.PostingDatetime = DateTime.Now;
                    answer.ModificationDatetime = DateTime.Now;
                    answer.UpvoteCount = 0;
                    answer.DownvoteCount = 0;
                    answer.Status = true;
                    
                    db.Answers.Add(answer);
                    db.SaveChanges();

                    //find answer creator
                    User resultUser = (from u in db.Users
                                       where u.ID == answer.CreatorID
                                       select u).FirstOrDefault();
                    resultUser.AnswerCount++;
                    db.SaveChanges();

                    Session["User"] = resultUser;
                }
            }

            return RedirectToAction("ViewQuestion", "Question", new { ID = postAnswer.QuestionID });
        }

        public ActionResult EditAnswer(int? ID)
        {
            if (ID == null)
                return View("Error");

            User user = (User)Session["User"];
            if (user != null)
            {
                Answer answer = (from a in db.Answers
                                 where a.ID == ID
                                 select a).SingleOrDefault();

                ViewBag.AnswerID = answer.ID;
                ViewBag.Content = answer.Content;

                return View();
            }

            return RedirectToAction("Login", "User");
        }
        
        [HttpPost]
        public ActionResult EditAnswer(EditAnswer editAnswer)
        {
            User user = (User)Session["User"];
            if (user != null)
            {
                if (ModelState.IsValid)
                {
                    Answer answer = (from a in db.Answers
                                     where a.ID == editAnswer.ID
                                     select a).SingleOrDefault();

                    answer.Content = editAnswer.Content;
                    answer.ModificationDatetime = DateTime.Now;
                    db.SaveChanges();

                    return RedirectToAction("ViewQuestion", "Question", new { ID = answer.QuestionID });
                }

                ViewBag.AnswerID = editAnswer.ID;
                ViewBag.Content = editAnswer.Content;

                return View();
            }

            return RedirectToAction("Login", "User");
        }
        
        public ActionResult DeleteAnswer(int? ID)
        {
            if (ID == null)
                return View("Error");

            User user = (User)Session["User"];
            if (user != null)
            {
                Answer answer = (from a in db.Answers
                                 where a.ID == ID
                                 select a).SingleOrDefault();

                int questionID = answer.QuestionID;

                //decrease user's answer count
                User resultUser = (from u in db.Users
                                   where u.ID == answer.CreatorID
                                   select u).FirstOrDefault();
                
                resultUser.AnswerCount--;
                db.Answers.Remove(answer);
                db.SaveChanges();

                Session["User"] = resultUser;

                return RedirectToAction("ViewQuestion", "Question", new { ID = questionID });
            }

            return RedirectToAction("Login", "User");
        }

        [HttpPost]
        public JsonResult Vote(VoteModel data)
        {
            User sessionUser = (User)Session["User"];

            Answer answer = (from a in db.Answers
                             where a.ID == data.AnswerID
                             select a).SingleOrDefault();

            Vote vote = (from v in db.Votes
                        where v.AnswerID == data.AnswerID && v.UserID == sessionUser.ID
                        select v).SingleOrDefault();

            if (vote == null)
            {
                Vote newVote = new Vote();
                newVote.AnswerID = data.AnswerID;
                newVote.UserID = sessionUser.ID;
                newVote.Status = data.VoteType;
                db.Votes.Add(newVote);

                if (data.VoteType)
                {
                    answer.UpvoteCount++;
                }
                else
                {
                    answer.DownvoteCount++;
                }
            }
            else
            {
                if (vote.Status != data.VoteType)
                {
                    vote.Status = data.VoteType;

                    if (data.VoteType)
                    {
                        answer.UpvoteCount++;
                        answer.DownvoteCount--;
                    }
                    else
                    {
                        answer.UpvoteCount--;
                        answer.DownvoteCount++;
                    }
                }
                else
                {
                    db.Votes.Remove(vote);

                    if (data.VoteType)
                    {
                        answer.UpvoteCount--;
                    }
                    else
                    {
                        answer.DownvoteCount--;
                    }
                }
            }
            db.SaveChanges();

            return Json(new { answer.UpvoteCount, answer.DownvoteCount });
        }

        public ActionResult ReportAnswer(int? ID)
        {
            if (ID == null)
                return View("Error");

            ViewBag.AnswerID = ID;
            return View();
        }

        [HttpPost]
        public ActionResult ReportAnswer(int AnswerID, string Reason)
        {
            User sessionUser = (User)Session["User"];

            AnswerReport answerReport = new AnswerReport
            {
                AnswerID = AnswerID,
                ReporterID = sessionUser.ID,
                Reason = Reason,
                ReportDatetime = DateTime.Now,
                Status = true
            };

            db.AnswerReports.Add(answerReport);

            try
            {
                db.SaveChanges();

                Answer answer = (from a in db.Answers
                                 where a.ID == AnswerID
                                 select a).SingleOrDefault();
                User resultUser = (from u in db.Users
                                   where u.ID == answer.CreatorID
                                   select u).SingleOrDefault();
                resultUser.ReportCount++;
                db.SaveChanges();

                return RedirectToAction("ViewQuestion", "Question", new { ID = answer.QuestionID });
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "You have already reported this answer");
                ViewBag.AnswerID = AnswerID;
                return View();
            }

        }

        public ActionResult ChangeAnswerStatus(int? ID)
        {
            if (ID == null)
                return View("Error");
            
            Answer answer = (from a in db.Answers
                             where a.ID == ID
                             select a).SingleOrDefault();
            answer.Status = !answer.Status;
            db.SaveChanges();
            return RedirectToAction("ViewQuestion", "Question", new { ID = answer.QuestionID });
        }

        public ActionResult ReportedAnswer(int? ID)
        {
            if (ID == null)
                return View("Error");

            Answer answer = (from a in db.Answers
                             where a.ID == ID
                             select a).SingleOrDefault();

            List<AnswerCard> answerCards = new List<AnswerCard>();

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

            ViewBag.AnswerCards = answerCards;

            return View();
        }

        [HttpPost]
        public JsonResult ChangeReportStatus(int AnswerReportID)
        {
            User sessionUser = (User)Session["User"];

            AnswerReport answerReport = (from ar in db.AnswerReports
                                         where ar.ID == AnswerReportID
                                         select ar).SingleOrDefault();
            if (answerReport.Status)
            {
                answerReport.ReportHandlerID = sessionUser.ID;
            }
            else
            {
                answerReport.ReportHandlerID = null;
            }
            answerReport.Status = !answerReport.Status;
            db.SaveChanges();

            if (answerReport.Status)
            {
                return Json(new { ReportStatus = "Pending", ReportHandlerID = -1, ReportHandlerName = "" });
            }
            else
            {
                User reportHandler = (from u in db.Users
                                      where u.ID == answerReport.ReportHandlerID
                                      select u).SingleOrDefault();

                return Json(new { ReportStatus = "Resolved", ReportHandlerID = reportHandler.ID, ReportHandlerName = reportHandler.Name });
            }

        }

    }
}