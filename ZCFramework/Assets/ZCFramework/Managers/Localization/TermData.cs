

namespace ZCFrame
{
    public struct TermData
    {
        public string Term { get; private set; }
        public string[] Languages { get; private set; }


        public TermData(string term, string[] languages)
        {
            Term = term;
            Languages = languages;
        }

    }
}


