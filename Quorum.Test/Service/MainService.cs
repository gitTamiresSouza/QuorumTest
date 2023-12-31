using ChoETL;
using Quorum.Test.Models;
using System.Reflection.Emit;

namespace Quorum.Test.Service
{
    public class MainService
    {
        private readonly IConfiguration _configuration;
        public MainService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task ProcessData()
        {
            var bills = new List<Bills>();
            var legislators = new List<Legislators>();
            var voteResults = new List<VoteResults>();

            var lines = File.ReadAllLines($"{_configuration.GetSection($"InputCSVsPath").Value}/bills.csv").Skip(1);
            foreach (var line in lines)
                bills.Add(new Bills
                {
                    Id = Convert.ToInt32(line.Split(',')[0]),
                    Title = line.Split(",")[1],
                    PrimarySponsor = Convert.ToInt32(line.Split(',')[2])
                });

            lines = File.ReadAllLines($"{_configuration.GetSection($"InputCSVsPath").Value}/legislators.csv").Skip(1);
            foreach (var line in lines)
                legislators.Add(new Legislators
                {
                    Id = Convert.ToInt32(line.Split(',')[0]),
                    Name = line.Split(",")[1]
                });

            lines = File.ReadAllLines($"{_configuration.GetSection($"InputCSVsPath").Value}/vote_results.csv").Skip(1);
            foreach (var line in lines)
                voteResults.Add(new VoteResults
                {
                    Id = Convert.ToInt32(line.Split(',')[0]),
                    LegislatorId = Convert.ToInt32(line.Split(',')[1]),
                    VoteId = Convert.ToInt32(line.Split(',')[2]),
                    VoteType = Convert.ToInt32(line.Split(',')[3])
                });

            var deliverables1 = new List<LegislatorsSupportOppose>();
            legislators.ForEach(r => deliverables1.Add(new LegislatorsSupportOppose()
            {
                Id = r.Id,
                Name = r.Name,
                NumOpposedBills = voteResults.Count(x => x.LegislatorId == r.Id && x.VoteType == 2),
                NumSupportedBills = voteResults.Count(x => x.LegislatorId == r.Id && x.VoteType == 1),
            }));
            using (var parser = new ChoCSVWriter($"{_configuration.GetSection($"OutputCSVsPath").Value}/legislators-support-oppose-count.csv"))
                parser.Write(deliverables1);

            var deliverables2 = new List<BillsOutput>();
            bills.ForEach(r => deliverables2.Add(new BillsOutput()
            {
                Id = r.Id,
                Title = r.Title,
                OpposerCount = voteResults.Count(x => x.VoteType == 2),
                SupporterCount = voteResults.Count(x => x.VoteType == 1),
                PrimarySponsor = legislators.FirstOrDefault(x => x.Id == r.PrimarySponsor) == null ? "Unknown" : legislators.FirstOrDefault(x => x.Id == r.PrimarySponsor).Name
            }));
            using (var parser = new ChoCSVWriter($"{_configuration.GetSection($"OutputCSVsPath").Value}/bills.csv"))
                parser.Write(deliverables2);
        }
    }
}
