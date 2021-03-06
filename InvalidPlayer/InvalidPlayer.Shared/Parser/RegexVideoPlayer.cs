﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using InvalidPlayerCore.Container;
using InvalidPlayerCore.Parser;
using InvalidPlayerCore.Parser.Attributes;

#pragma warning disable 649

namespace InvalidPlayer.Parser
{
    [Singleton("RegexVideoPlayer")]
    public class RegexVideoPlayer : WebVideoPlayer
    {
        [Inject]
        private List<IVideoParser> _parserList;

        private Dictionary<Regex, IVideoParser> _parsers;

        private string _url;

        public override string Url
        {
            get { return _url; }
            set { SetCurrentParser(_url = value); }
        }

        private void SetCurrentParser(string url)
        {
            Debug.WriteLine("weburl:{0}", url);
            foreach (var entry in from entry in _parsers let regex = entry.Key where regex.IsMatch(url) select entry)
            {
                Debug.WriteLine("support url,selected parser is :{0}", entry.Value);
                Parser = entry.Value;
                break;
            }
        }

        [Init]
        private void InitParser()
        {
            _parsers = new Dictionary<Regex, IVideoParser>(_parserList.Count);
            foreach (var parser in _parserList)
            {
                var patterns = parser.GetType().GetTypeInfo().GetCustomAttributes<WebUrlPatternAttribute>();
                foreach (var pattern in patterns)
                {
                    Debug.WriteLine("register parser: {0} {1}", pattern.Pattern, parser);
                    _parsers.Add(new Regex(pattern.Pattern), parser);
                }
            }
        }
    }
}