using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using JetBrains.Annotations;
using XyrusWorx.Collections;
using XyrusWorx.Diagnostics;
using XyrusWorx.SchemaBrowser.Business;
using XyrusWorx.SchemaBrowser.Business.ObjectModel;

namespace XyrusWorx.SchemaBrowser.Windows.Services
{
    [UsedImplicitly(ImplicitUseTargetFlags.Itself)]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class SchemaLoader
    {
        private readonly ILogWriter mSyslog;
        private readonly LocalizationService mLocalizationService;
        private readonly XmlIndex mIndex;

        public SchemaLoader(ILogWriter syslog, LocalizationService localizationService)
        {
            mSyslog = syslog;
            mLocalizationService = localizationService;
            mIndex = new XmlIndex(syslog);
        }

        public async Task<IResult> LoadAsync(string pathOrUrl) => await Task.Run(() => Load(pathOrUrl));
        public IResult Load(string pathOrUrl)
        {
            mIndex.ClearContext();
            return mIndex.Process(pathOrUrl);
        }
        
        // ReSharper disable once UnusedMember.Global
        public async Task<IResult> AppendAsync(string pathOrUrl) => await Task.Run(() => Append(pathOrUrl));
        public IResult Append(string pathOrUrl)
        {
            return mIndex.Process(pathOrUrl);
        }
		
        public async IAsyncEnumerable<ComplexTypeModel> GetRootsAsync()
        {
            foreach (var name in GetRootNames())
            {
                var processor = CreateProcessor();
                await Task.Run(() => processor.Process(name));
                yield return processor.GetOutput();
            }
        }
        
        // ReSharper disable once UnusedMember.Global
        public IEnumerable<ComplexTypeModel> GetRoots()
        {
            foreach (var name in GetRootNames())
            {
                var processor = CreateProcessor();
                processor.Process(name);
                yield return processor.GetOutput();
            }
        }

        private Processor CreateProcessor()
        {
            var processor = new Processor(mIndex, mLocalizationService, mSyslog);
            processor.Context.OutputLanguage = null;
            processor.Context.AnnotationLanguage = null;
            return processor;
        }
        private IEnumerable<XName> GetRootNames()
        {
            var complexTypeDictionary = new Dictionary<XName, XmlIndexEntry>();
			
            foreach (var entry in mIndex.GetEntries("complexType") ?? new XmlIndexEntry[0])
            {
                complexTypeDictionary.AddIfMissing(entry.Name, entry);
            }

            return
                from nameEntryPair in complexTypeDictionary
                where mIndex.IsRootTypeCandidate(nameEntryPair.Key)
                select nameEntryPair.Key;
        }
    }
}