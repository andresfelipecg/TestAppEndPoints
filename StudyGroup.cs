using System;
using System.Collections.Generic;

namespace TestApp
{
    public class StudyGroup
    {
        public StudyGroup(int studyGroupId, string name, Subject subject, DateTime createDate, List<User> users)
        {
            StudyGroupId = studyGroupId;
            Name = name;
            Subject = subject;
            CreateDate = createDate;
            Users = users;
        }
        //Some logic will be missing to validate values according to acceptance criteria, but imagine it is existing or do it yourself
        public int StudyGroupId { get; set; }

        public string Name { get; set; }

        public Subject Subject { get; set; }

        public DateTime CreateDate { get; set; }

        public List<User> Users { get;  set; }

        public void AddUser(User user)
        {
            Users.Add(user);
        }

        public void RemoveUser(User user)
        {
            Users.Remove(user);
        }
    }

    public enum Subject
    {
        Math,
        Chemistry,
        Physics
    }

    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}

