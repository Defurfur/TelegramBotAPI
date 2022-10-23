namespace ReaSchedule.Models
{
    public class ReaClass : IIdentifyable<int>
    {
        public int Id { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string ClassType { get; set; } = string.Empty;
        public string OrdinalNumber { get; set; } = string.Empty;
        public string Audition { get; set; } = string.Empty;
        public string Professor { get; set; } = string.Empty;
        public string Subgroup { get; set; } = string.Empty;


    }

    public static class ClassOrdinalNumber
    {
        public static string firstClass { get { return "1-я пара"; } }
        public static string secondClass { get { return "2-я пара"; } }
        public static string thirdClass { get { return "3-я пара"; } }
        public static string fourthClass { get { return "4-я пара"; } }
        public static string fifthClass { get { return "5-я пара"; } }
        public static string sixthClass { get { return "6-я пара"; } }
        public static string seventhClass { get { return "7-я пара"; } }
        public static string eigthClass { get { return "8-я пара"; } }
    }

    //    public static class ClassType
    //    {
    //        public static string practical { get { return "Практическое занятие"; } }
    //        public static string lecture { get { return "Лекция"; } }
    //        public static string lab { get { return "Лабораторная работа"; } }
    //        public static string exam { get { return "Экзамен"; } }
    //        public static string credit { get { return "Зачет"; } }
    //        public static string diffCredit { get { return "Дифференцированный зачет"; } }
    //    }
}