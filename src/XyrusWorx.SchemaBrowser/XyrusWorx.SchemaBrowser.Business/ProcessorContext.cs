using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using JetBrains.Annotations;
using XyrusWorx.Diagnostics;
using XyrusWorx.SchemaBrowser.Business.ObjectModel;

namespace XyrusWorx.SchemaBrowser.Business
{
	public class ProcessorContext
	{
		private readonly LocalizationService mLocalizationService;
		private readonly XmlIndex mIndex;
		private readonly ILogWriter mLog;

		private readonly ConcurrentDictionary<Type, ConcurrentDictionary<XName, object>> mCache;
		private readonly ConcurrentDictionary<int, Stack<XElement>> mElementStacks;
		
		private IStringResolver mOutputLanguageResolver;
		
		private string mOutputLanguage;
		private string mAnnotationLanguage;

		public ProcessorContext([NotNull] XmlIndex index, [NotNull] LocalizationService localizationService, ILogWriter log = null)
		{
			mIndex = index ?? throw new ArgumentNullException(nameof(index));
			mLocalizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
			mLog = log ?? new NullLogWriter();
			
			mElementStacks = new ConcurrentDictionary<int, Stack<XElement>>();
			mCache = new ConcurrentDictionary<Type, ConcurrentDictionary<XName, object>>();
			mOutputLanguageResolver = localizationService.GetOutputLanguage(CultureInfo.InvariantCulture);
		}

		public string AnnotationLanguage
		{
			get => mAnnotationLanguage;
			set => mAnnotationLanguage = value;
		}
		public string OutputLanguage
		{
			get => mOutputLanguage;
			set
			{
				mOutputLanguage = value;
				mOutputLanguageResolver = string.IsNullOrWhiteSpace(value)
					? mLocalizationService.GetOutputLanguage(CultureInfo.InvariantCulture)
					: mLocalizationService.GetOutputLanguage(value);
			}
		}

		[NotNull]
		public XmlIndex Index => mIndex;
		
		[NotNull]
		public ILogWriter Log => mLog;
		
		[NotNull]
		public T Init<T>([NotNull] XName qualifiedName) where T: INamedModel, new() 
			=> (T)GetCache(typeof(T)).GetOrAdd(qualifiedName, x => new T() { TypeName = qualifiedName });
		
		public bool Init<T>([NotNull] XName qualifiedName, out T model) where T: INamedModel, new()
		{
			var exists = GetCache(typeof(T)).TryGetValue(qualifiedName, out var obj);
			if (!exists)
			{
				model = Init<T>(qualifiedName);
				return false;
			}

			model = (T)obj;
			return true;
		}
		public bool Has<T>([NotNull] XName qualifiedName) where T: INamedModel, new() 
			=> GetCache(typeof(T)).TryGetValue(qualifiedName, out _);

		[NotNull]
		public IEnumerable<T> All<T>() => GetCache(typeof(T)).Values.OfType<T>();

		[NotNull]
		public ConcurrentDictionary<XName, object> GetCache([NotNull] Type type) 
			=> mCache.GetOrAdd(type, x => new ConcurrentDictionary<XName, object>());

		public void Stack([NotNull] Action<Stack<XElement>> action)
			=> action.Invoke(mElementStacks.GetOrAdd(Thread.CurrentThread.ManagedThreadId, x => new Stack<XElement>()));
		
		public T Stack<T>([NotNull] Func<Stack<XElement>, T> func)
			=> func.Invoke(mElementStacks.GetOrAdd(Thread.CurrentThread.ManagedThreadId, x => new Stack<XElement>()));

		public void Read<T>(object model) where T : IXsdParticle, new()
		{
			var particle = new T();
			particle.Process(this, model);
		}
		public TOut Read<TParticle, TOut>(XName qualifiedName) 
			where TParticle : IXsdParticle, new()
			where TOut: TargetableModel, INamedModel, new()
		{
			if (!Init<TOut>(qualifiedName, out var model))
			{
				Read<TParticle>(model);
			}
			
			return model;
		}

		[NotNull]
		public IDisposable For(XElement element) => new Scope(() => Push(element), () => Pop()).Enter();

		[NotNull]
		public IStringResolver GetOutputLanguageResolver() => mOutputLanguageResolver;

		public void Push(XElement element) => Stack(x => x.Push(element));
		public XElement Peek() => Stack(x => x.Peek());
		public XElement Pop() => Stack(x => x.Pop());
	}
}