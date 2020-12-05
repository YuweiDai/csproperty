using System;
using CSCZJ.Core;
using CSCZJ.Services.Events;
using CSCZJ.Core.Data;
using CSCZJ.Core.Caching;
using System.Linq;
using CSCZJ.Data;
using CSCZJ.Services.Property;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq.Expressions;
using CSCZJ.Core.Domain.Properties;
using System.Data.Entity.Spatial;
using System.Reflection;
using System.Collections;

namespace CSCZJ.Services.Properties
{
    public class PropertyService : IPropertyService
    {
        private const string PROPERTY_BY_ID_KEY = "CSCZJ.property.id-{0}";
        private const string PROPERTIES_PATTERN_KEY = "CSCZJ.property.";

        private const string xqregion = "MULTIPOLYGON (((118.83061773800011 28.984037100000023, 118.83027875800008 28.97749847800003, 118.83019018700008 28.974613994000038, 118.8305129040001 28.972312430000045, 118.83115703500005 28.970457877000058, 118.83322749800004 28.966178772000035, 118.83465902600005 28.963238638000064, 118.83466298400003 28.963230511000063, 118.83583213100007 28.960829266000076, 118.83686879600009 28.958705528000053, 118.83732194300001 28.957777200000066, 118.83775487000003 28.956890297000029, 118.83803179300003 28.956322986000032, 118.83855440900004 28.955252340000072, 118.83884254200007 28.954662066000026, 118.84020364700007 28.951528287000031, 118.84060934700005 28.948819378000053, 118.84028735600009 28.945962245000032, 118.84021588600001 28.945591322000041, 118.84017773300002 28.945393320000051, 118.84001737400001 28.944561079000039, 118.8465220380001 28.944563016000075, 118.85055186600005 28.945061308000049, 118.85285442800011 28.946059412000068, 118.85467031100006 28.94780232200003, 118.85597205100009 28.950818167000079, 118.85607345800008 28.95135833300003, 118.85650909700007 28.95367883800003, 118.85669442000005 28.954665995000028, 118.85691678400008 28.955533511000056, 118.85699048400011 28.955821042000025, 118.85767353400001 28.958485857000028, 118.85814994500004 28.960344497000051, 118.85944800100003 28.964676319000034, 118.86021977400003 28.967979994000075, 118.86040178500002 28.969061312000065, 118.8606690360001 28.970019251000053, 118.86165747200005 28.971703474000037, 118.86217721200001 28.972589070000026, 118.8622569690001 28.972724968000023, 118.86229325900001 28.972786799000062, 118.86231377600006 28.97282175600003, 118.86233427800005 28.972856686000057, 118.86237058800009 28.972918555000035, 118.86546466900006 28.978391817000045, 118.86686591500006 28.980363157000056, 118.86924732900002 28.982537708000052, 118.87079458700009 28.983395469000072, 118.87090955700012 28.98343731500006, 118.8713690510001 28.983604557000035, 118.87058289800007 28.985239283000055, 118.86954085900004 28.987714383000025, 118.86940295200009 28.988812706000033, 118.87045117500008 28.992033157000037, 118.86934025900007 28.993799737000074, 118.86782612500008 28.995633482000073, 118.86706796600004 28.996669661000055, 118.8659381440001 28.996595283000033, 118.86269051200009 28.995498156000053, 118.86091137900007 28.994901580000032, 118.85981421800011 28.994533682000053, 118.85758987400004 28.993787819000033, 118.85758166700009 28.993785067000033, 118.85496771900011 28.992908563000071, 118.85288624200007 28.992210606000071, 118.84674147700002 28.99020600700004, 118.8438976000001 28.989421495000045, 118.83255582700008 28.987639510000065, 118.83230285500008 28.987556985000026, 118.83206241800008 28.987465254000028, 118.83130982700004 28.987073390000035, 118.83070544000009 28.986148014000037, 118.83061773800011 28.984037100000023)))";
        private const string jjqregion = "MULTIPOLYGON (((118.88477295200005 28.93451525100005, 118.88488604700001 28.934363443000052, 118.8849706530001 28.934249876000024, 118.89526449900006 28.920432414000061, 118.89964226400002 28.914173855000058, 118.89614908800002 28.914121941000076, 118.88459716000011 28.914763252000057, 118.87811260400008 28.915212586000052, 118.8689155080001 28.916345943000067, 118.86405046700008 28.916570947000025, 118.8603196470001 28.916596446000028, 118.85774818100003 28.916386991000024, 118.85646065900005 28.91583406500007, 118.8545084000001 28.91461408300006, 118.85247788900006 28.914179473000047, 118.84975451100001 28.914751369000044, 118.84643043700009 28.915550784000061, 118.83793597800002 28.918321083000023, 118.82918880700004 28.910440974000039, 118.82251836100011 28.902381993000063, 118.8166223720001 28.893678435000027, 118.83119228300006 28.885017819000041, 118.83264901900009 28.884642891000055, 118.83169354000006 28.883675702000062, 118.82972154000004 28.881192706000036, 118.82851156000004 28.879454544000055, 118.82613783000011 28.876350436000052, 118.82579559100009 28.875616738000076, 118.82261744200002 28.869601259000035, 118.82093782300001 28.866681554000024, 118.82006041000011 28.865578739000057, 118.81890619800004 28.864602149000063, 118.81644270800007 28.862711005000051, 118.81344750500011 28.86051481100003, 118.81488916400008 28.858007938000071, 118.81753764600001 28.853430360000061, 118.81936802100006 28.853287823000073, 118.82588779300011 28.853898445000027, 118.82574680100004 28.852549620000048, 118.8262279700001 28.852530349000062, 118.82701354900007 28.852590892000023, 118.82783572100004 28.852737406000074, 118.8284101480001 28.852839771000049, 118.82924157800005 28.852954405000048, 118.82998547800003 28.853042405000053, 118.83129895400009 28.853173449000053, 118.83236514400005 28.85317201600003, 118.83336401600002 28.853070862000038, 118.83409423000001 28.852991247000034, 118.83623250100004 28.852474516000029, 118.8385663470001 28.851825732000066, 118.83981456100003 28.85151223500003, 118.84206464300007 28.851288105000037, 118.84423855400007 28.851536003000035, 118.84654713300006 28.852271801000029, 118.84770273600009 28.852711391000071, 118.84992947000001 28.853606270000057, 118.85179551100009 28.854094255000064, 118.8536178170001 28.854108084000075, 118.85495069500007 28.854093052000053, 118.85829961100001 28.854053686000043, 118.85995537100007 28.854174478000061, 118.86101820600004 28.854509042000075, 118.86209465700006 28.85479852800006, 118.86350626600006 28.855343173000051, 118.86559327800001 28.85615642700003, 118.86697357900005 28.856686473000025, 118.86787435800011 28.857033839000053, 118.87233152900001 28.85840365200005, 118.87851828800001 28.859980889000042, 118.88579466500005 28.862091520000035, 118.88686066500009 28.862383313000066, 118.89060119600003 28.862611775000062, 118.89615602900005 28.862088038000024, 118.89898955300009 28.86170493700007, 118.90307622300008 28.860526374000074, 118.91125245700005 28.861311887000056, 118.92458414500004 28.862031715000057, 118.92574688600007 28.862041001000023, 118.93252711700006 28.862700177000079, 118.96360482500006 28.863183434000064, 118.97772070100007 28.864032264000059, 118.98602354000002 28.868179110000028, 118.99546799100006 28.873899941000047, 119.01293909100002 28.887597021000033, 119.02377511000009 28.89664450500004, 119.03076269900009 28.901107380000042, 119.03523771400012 28.90038620100006, 119.03603844700001 28.906670157000065, 119.03695906600001 28.915858118000074, 119.04022516600003 28.916755263000027, 119.04241171900003 28.917883046000043, 119.04613872200002 28.919855751000057, 119.04633555300006 28.920084664000058, 119.04771305100007 28.922191015000067, 119.04875691400002 28.922533181000063, 119.04856106700004 28.926977122000039, 119.04846185000008 28.929201267000053, 119.04850653700009 28.929700707000052, 119.04849886400007 28.930850627000041, 119.04846479200012 28.931445723000024, 119.04849565000006 28.932272233000049, 119.04851844600012 28.934410992000039, 119.04848005200006 28.934625046000065, 119.04824503600003 28.935153204000073, 119.04749338800002 28.936571750000041, 119.04674180900008 28.938042986000028, 119.04645245100005 28.938515296000048, 119.04527825900004 28.939623501000028, 119.04463265000004 28.940294299000072, 119.04454128400005 28.940586544000041, 119.04440196900009 28.94125563800003, 119.04391527600001 28.944016752000039, 119.04359113600003 28.947302991000072, 119.04343898500008 28.948008730000026, 119.04305500000009 28.94863618200003, 119.04201556500004 28.951196972000048, 119.03917935600009 28.957734113000072, 119.03405629500003 28.96967593100004, 119.03125893600009 28.976356517000056, 119.03019340700007 28.97877139600007, 119.02849820800009 28.979836869000053, 119.02739295900005 28.981701410000028, 119.02075232200002 28.984892539000043, 119.01914187200009 28.985560471000042, 119.01083607800001 28.987109169000064, 119.00155137900003 28.987691514000062, 118.99565164800003 28.992244729000049, 118.99263996800005 28.994073700000058, 118.99088931900008 28.994648210000037, 118.98714257600011 28.994902188000026, 118.98673299400002 28.992861515000072, 118.98098833400002 28.990968593000048, 118.97841923200008 28.987819628000068, 118.97687579000001 28.987052076000055, 118.97405571800005 28.985987238000064, 118.9767390400001 28.977687425000056, 118.95054975000005 28.971152914000072, 118.94313604200011 28.96721831900004, 118.93681254100011 28.962544109000078, 118.92142564700009 28.951580428000057, 118.9186565760001 28.950149152000051, 118.91662096800007 28.950042453000037, 118.90204695600005 28.95190875000003, 118.8963119340001 28.952802070000075, 118.88295347200005 28.954991454000037, 118.87718327100004 28.954345549000038, 118.87706656100011 28.954332484000076, 118.87633296400008 28.954250367000043, 118.87563339900009 28.95417205900003, 118.87549967000007 28.952233633000048, 118.87538319900011 28.950545378000072, 118.87532426200005 28.949550578000071, 118.87563525000007 28.948580363000076, 118.87598606000006 28.947485908000033, 118.87603547500009 28.947331746000032, 118.87651328000004 28.946162358000038, 118.87732865200007 28.944621430000041, 118.8776402530001 28.944032555000035, 118.87795462500003 28.943443307000052, 118.87842787400007 28.942556266000054, 118.87857413500001 28.942278411000075, 118.87870754700009 28.94202496500003, 118.87900794600012 28.94145429200006, 118.8791545790001 28.941225423000049, 118.87962474100004 28.940491579000025, 118.87976987700006 28.940342830000077, 118.88206792800008 28.937987584000041, 118.8823129110001 28.937685972000054, 118.88337610800011 28.936377008000079, 118.88477295200005 28.93451525100005)))";
        private const string lcqregion = "MULTIPOLYGON (((118.85129157100005 28.942509139000038, 118.85061844300003 28.940104038000072, 118.85032623200004 28.938541301000043, 118.85003437100011 28.93763840400004, 118.85003435700003 28.937638366000044, 118.85002932100008 28.937622786000077, 118.85002506500007 28.937609619000057, 118.85002411300002 28.937606674000051, 118.84985688100005 28.93708932100003, 118.84943080700009 28.935782152000058, 118.84856268900012 28.933732144000032, 118.84764124700007 28.932189544000039, 118.84709641000006 28.931313481000075, 118.84694233000005 28.931065728000078, 118.84619278300011 28.929860503000043, 118.84747168400008 28.929757272000074, 118.84837864300005 28.929685368000037, 118.84839658600004 28.929683946000068, 118.85089168800005 28.929486138000073, 118.85268269700009 28.929349140000056, 118.85276256100008 28.92934303100003, 118.85341727200012 28.929292951000036, 118.85543278300008 28.929245249000076, 118.85718178400009 28.929379603000029, 118.85981124000011 28.929729830000042, 118.85981127600007 28.929729835000046, 118.85992245500006 28.929744506000077, 118.85992951500009 28.929745437000065, 118.86245220300009 28.93007837600004, 118.86245741200003 28.93007906400004, 118.86639648300002 28.930600785000024, 118.86800453000001 28.930812682000067, 118.87010887800011 28.931089978000045, 118.87121559500008 28.931239093000045, 118.87351133400011 28.931548411000051, 118.87608641100007 28.931895367000038, 118.87775883000006 28.932110669000053, 118.87897113100007 28.932266737000077, 118.88185836300011 28.932833692000031, 118.88327375100005 28.933496490000039, 118.88477246000002 28.934514916000069, 118.88337610800011 28.936377008000079, 118.88206792800008 28.937987584000041, 118.87962474100004 28.940491579000025, 118.87900794600012 28.94145429200006, 118.87870754700009 28.94202496500003, 118.87857413500001 28.942278411000075, 118.87842787400007 28.942556266000054, 118.87795462500003 28.943443307000052, 118.8776402530001 28.944032555000035, 118.87747416100001 28.944345931000043, 118.87651946400001 28.946147223000025, 118.87603547500009 28.947331746000032, 118.87598606000006 28.947485908000033, 118.87532426200005 28.949550578000071, 118.87538319900011 28.950545378000072, 118.87563339900009 28.95417205900003, 118.87633296400008 28.954250367000043, 118.87706656100011 28.954332484000076, 118.87769184700005 28.954402477000031, 118.87784626400003 28.954419763000033, 118.87791138600005 28.954538549000063, 118.87826436400007 28.955182402000048, 118.87846270000011 28.956874709000033, 118.87859634800009 28.957607732000042, 118.87868482900001 28.958093033000068, 118.87909958700004 28.96027428900004, 118.87928841600001 28.960832141000026, 118.88097957600007 28.963953768000067, 118.88211330500008 28.96603525200004, 118.88269453800001 28.967102371000067, 118.88279407800007 28.967285124000057, 118.88315357500005 28.967945146000034, 118.88343334100011 28.968446166000035, 118.8849403480001 28.971145003000061, 118.88561635400004 28.972405041000059, 118.88608077200001 28.973270692000028, 118.88633247200005 28.974912391000032, 118.88639591300011 28.975666467000053, 118.88572615500004 28.975774041000079, 118.88543084300011 28.975836848000029, 118.88492745200006 28.975943909000023, 118.8842531680001 28.976111318000051, 118.88365198200006 28.976349447000075, 118.88304664900011 28.976611489000049, 118.88235741300002 28.977085017000036, 118.88001649400007 28.978828586000077, 118.88001594500008 28.978829182000027, 118.88001470800009 28.978830769000069, 118.88000986700001 28.978836987000079, 118.87980701900005 28.979097370000034, 118.87960308000004 28.979397173000052, 118.87925077300008 28.97997173400006, 118.87914064000006 28.980151344000035, 118.87867746800009 28.980975278000074, 118.87867744700009 28.980975346000037, 118.87852133400008 28.981475746000058, 118.87808732700012 28.983214732000079, 118.87787359400011 28.983743490000052, 118.87768552000011 28.983892320000052, 118.8755325950001 28.984252826000045, 118.87504033000005 28.984306196000034, 118.87450498400005 28.984320922000052, 118.87399022200009 28.984317507000071, 118.87348216900011 28.984253677000027, 118.87308424200012 28.984170180000035, 118.87297482000008 28.984140840000066, 118.87268272600011 28.984062519000076, 118.87260615200012 28.984035998000024, 118.87197001900006 28.983815676000063, 118.8713690510001 28.983604557000035, 118.87090955700012 28.98343731500006, 118.87079458700009 28.983395469000072, 118.86924732900002 28.982537708000052, 118.86686591500006 28.980363157000056, 118.8655810250001 28.97855551300006, 118.86546466900006 28.978391817000045, 118.86237058800009 28.972918555000035, 118.86233427700006 28.972856686000057, 118.86231377600006 28.97282175600003, 118.86229325900001 28.972786799000062, 118.8622569690001 28.972724968000023, 118.86217721200001 28.972589070000026, 118.86165747000007 28.971703475000027, 118.8612712370001 28.971045367000045, 118.8606690360001 28.970019251000053, 118.8605744460001 28.969680203000053, 118.86040178500002 28.969061312000065, 118.86028382300003 28.968360508000046, 118.86021977400003 28.967979994000075, 118.85944800100003 28.964676319000034, 118.85880764100011 28.962539335000031, 118.85814994500004 28.960344497000051, 118.85767353400001 28.958485857000028, 118.85699048400011 28.955821042000025, 118.85691678400008 28.955533511000056, 118.85681642500003 28.955141979000075, 118.85669442000005 28.954665995000028, 118.85664342500002 28.954416690000073, 118.8565602760001 28.954010175000064, 118.85650909700007 28.95367883800003, 118.85607345800008 28.95135833300003, 118.85597205100009 28.950818167000079, 118.85467031100006 28.94780232200003, 118.85285442800011 28.946059412000068, 118.85231790400007 28.945826842000031, 118.85247536500003 28.945743609000033, 118.85248796600001 28.945632649000061, 118.85226424000007 28.945022114000039, 118.85203171700005 28.944387572000039, 118.85129157100005 28.942509139000038)))";
        private const string kcegion = "";
        private const string qjregion = "";

        private readonly IDbContext _context;
        private readonly IRepository<CSCZJ.Core.Domain.Properties.Property> _propertyRepository;
        private readonly IRepository<CSCZJ.Core.Domain.Properties.GovernmentUnit> _governmentUnitRepository;
        private readonly IRepository<PropertyPicture> _propertyPictureRepository;
        private readonly IRepository<PropertyFile> _propertyFileRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;


        public PropertyService(ICacheManager cacheManager, IDbContext context,
             IRepository<CSCZJ.Core.Domain.Properties.Property> propertyRepository, IRepository<CSCZJ.Core.Domain.Properties.GovernmentUnit> governmentUnitRepository,
             IRepository<PropertyPicture> propertyPictureRepository, IRepository<PropertyFile> propertyFileRepository,
        IEventPublisher eventPublisher)
        {

            this._cacheManager = cacheManager;

            _propertyRepository = propertyRepository;
            _governmentUnitRepository = governmentUnitRepository;

            _propertyPictureRepository = propertyPictureRepository;
            _propertyFileRepository = propertyFileRepository;

            this._eventPublisher = eventPublisher;

            this._context = context;
        }

        public void GetProperties(string wkt)
        {
            //var extent = DbGeography.FromText(wkt);

            //var p1 = _propertyRepository.Table.Where(p => p.Location.Intersects(extent));
        }

        public void DeleteProperty(CSCZJ.Core.Domain.Properties.Property property)
        {
            if (property == null)
                throw new ArgumentNullException("property is null");

            property.Deleted = true;
            UpdateProperty(property);
        }

        public IList<CSCZJ.Core.Domain.Properties.Property> GetAllProperties()
        {
            var query = _propertyRepository.Table;

            query = query.Where(m => !m.Deleted);

            // query = query.Where(s => s.Published);

            return query.ToList();
        }

        public IQueryable<CSCZJ.Core.Domain.Properties.Property> GetPropertiesByGovernmentId(IList<int> governmentIds)
        {
            var query = _propertyRepository.Table;
            query = query.Where(p =>!p.Deleted && governmentIds.Contains(p.Government.Id));
            return query;
        }

        public IQueryable<CSCZJ.Core.Domain.Properties.Property> GetAllProperties(IList<int> governmentIds, bool showHidden = false)
        {
            var query = from p in _propertyRepository.Table.AsNoTracking()
                        select p;

            if (governmentIds.Count > 0) query = query.Where(p => governmentIds.Contains(p.Government.Id));
            Expression<Func<CSCZJ.Core.Domain.Properties.Property, bool>> expression = p => !p.Deleted;

            if (!showHidden) expression = expression.And(p => p.Published && !p.Off); 

            query = query.Where(expression);

            return query;
        }

        public IPagedList<CSCZJ.Core.Domain.Properties.Property> GetAllProperties(IList<int> governmentIds,string search = "", int pageIndex = 0, int pageSize = int.MaxValue,
            bool showHidden = true, PropertyAdvanceConditionRequest advanceCondition = null, params PropertySortCondition[] sortConditions)
        {
            var query = _propertyRepository.Table.AsNoTracking().AsQueryable();

            if (governmentIds.Count > 0) query = query.Where(p => governmentIds.Contains(p.Government.Id));

            Expression<Func<CSCZJ.Core.Domain.Properties.Property, bool>> expression = p => !p.Deleted;

            if (!showHidden) expression = expression.And(p => p.Published && !p.Off);

            //不查询子资产
            expression = expression.And(p => p.ParentPropertyId == 0);

            //字符串查询
            if (!string.IsNullOrEmpty(search))
            {
                expression = expression.And(p => p.Name.Contains(search) || p.Address.Contains(search));
            }

            //搜索条件
            if (advanceCondition != null)
            {
                #region 高级搜索实现
                if (advanceCondition.GovernmentId > 0)
                {
                    if (advanceCondition.SerachParentGovernement)
                    {
                        var ids = _governmentUnitRepository.TableNoTracking.Where(g => g.ParentGovernmentId == advanceCondition.GovernmentId || g.Id == advanceCondition.GovernmentId).Select(g => g.Id).ToArray();

                        expression = expression.And(p => ids.Contains(p.Government.Id));
                    }
                    else
                    {
                        expression = expression.And(p => p.Government.Id == advanceCondition.GovernmentId);
                    }
                }

                if (advanceCondition.GovernmentTypes.Count != 0)
                    expression = expression.And(p => advanceCondition.GovernmentTypes.Contains((int)p.Government.GovernmentType));
                //query = query.Where(s => advanceCondition.PropertyType.Contains((int)s.Government.GovernmentType));

                #region 资产类别和建筑面积、土地面积挂钩
                Expression<Func<CSCZJ.Core.Domain.Properties.Property, bool>> houseAndLandExpression = null;

                #region 建筑面积区间集合
                Expression<Func<CSCZJ.Core.Domain.Properties.Property, bool>> constructRangeExpression = null;
                if (advanceCondition.ConstructArea.Count > 0)
                {
                    #region 遍历区间集合
                    foreach (var range in advanceCondition.ConstructArea)
                    {
                        var min = range[0];
                        var max = range[1];

                        if (max == 0) max = int.MaxValue;

                        if (min >= max) continue;

                        if (constructRangeExpression == null) constructRangeExpression = p => p.ConstructArea >= min && p.ConstructArea <= max;
                        else constructRangeExpression = constructRangeExpression.Or(p => p.ConstructArea >= min && p.ConstructArea <= max);
                    }
                    #endregion                 
                }

                #endregion

                #region 土地面积区间集合
                Expression<Func<CSCZJ.Core.Domain.Properties.Property, bool>> landRangeExpression = null;

                if (advanceCondition.LandArea.Count > 0)
                {
                    #region 遍历区间集合
                    foreach (var range in advanceCondition.LandArea)
                    {
                        var min = range[0];
                        var max = range[1];

                        if (max == 0) max = int.MaxValue;

                        if (min >= max) continue;

                        if (landRangeExpression == null) landRangeExpression = p => p.LandArea >= min && p.LandArea <= max;
                        else landRangeExpression = landRangeExpression.Or(p => p.LandArea >= min && p.LandArea <= max);
                    }
                    #endregion                 
                }
                #endregion

                if (advanceCondition.PropertyType.Count == 0)
                {
                    if (landRangeExpression != null)
                    {
                        if (houseAndLandExpression == null) houseAndLandExpression = landRangeExpression;
                        else houseAndLandExpression = houseAndLandExpression.Or(landRangeExpression);
                    }
                }
                else
                {

                    #region 房屋及房屋面积、土地面积
                    if (advanceCondition.PropertyType.Contains(0))
                    {
                        Expression<Func<CSCZJ.Core.Domain.Properties.Property, bool>> xpression = null;//p => p.PropertyType == CSCZJ.Core.Domain.Properties.PropertyType.House;

                        if (constructRangeExpression != null)
                            xpression = xpression.And(constructRangeExpression);

                        if (landRangeExpression != null)
                            xpression = xpression.And(landRangeExpression);

                        houseAndLandExpression = xpression;
                    }
                    #endregion

                    #region 土地及土地面积
                    if (advanceCondition.PropertyType.Contains(1) || advanceCondition.PropertyType.Contains(2))
                    {
                        if (advanceCondition.PropertyType.Contains(0)) advanceCondition.PropertyType.Remove(0);
                        Expression<Func<CSCZJ.Core.Domain.Properties.Property, bool>> xpression = p => advanceCondition.PropertyType.Contains((int)p.PropertyType);

                        if (landRangeExpression != null)
                            xpression = xpression.And(landRangeExpression);

                        if (houseAndLandExpression == null) houseAndLandExpression = xpression;
                        else houseAndLandExpression = houseAndLandExpression.Or(xpression);
                    }
                    #endregion
                }

                if (houseAndLandExpression != null) expression = expression.And(houseAndLandExpression);
                #endregion

                if (advanceCondition.Region.Count != 0)
                    expression = expression.And(p => advanceCondition.Region.Contains((int)p.Region));

                #region 土地证书情况
                if (!(advanceCondition.Certificate_Both && advanceCondition.Certificate_Construct &&
                            advanceCondition.Certificate_Land && advanceCondition.Certificate_None))
                {
                    Expression<Func<CSCZJ.Core.Domain.Properties.Property, bool>> ex = null;
                    //if (advanceCondition.Certificate_Both) ex = p => p.HasConstructID && p.HasLandID;
                    //if (advanceCondition.Certificate_Construct)
                    //{
                    //    if (ex == null) ex = p => p.HasConstructID && !p.HasLandID;
                    //    else ex = ex.Or(p => p.HasConstructID && !p.HasLandID);
                    //}
                    //if (advanceCondition.Certificate_Land)
                    //{
                    //    if (ex == null) ex = p => !p.HasConstructID && p.HasLandID;
                    //    else ex = ex.Or(p => !p.HasConstructID && p.HasLandID);
                    //}
                    //if (advanceCondition.Certificate_None)
                    //{
                    //    if (ex == null) ex = p => !p.HasConstructID && !p.HasLandID;
                    //    else ex = ex.Or(p => !p.HasConstructID && !p.HasLandID);
                    //}

                    if (ex != null) expression = expression.And(ex);
                }
                #endregion

                #region 使用现状
                if (!(advanceCondition.Current_Self && advanceCondition.Current_Rent && advanceCondition.Current_Lend && advanceCondition.Current_Idle))
                {
                    Expression<Func<CSCZJ.Core.Domain.Properties.Property, bool>> ex = null;
                    // if (advanceCondition.Current_Self) ex = p => p.CurrentUse_Self > 0;
                    if (advanceCondition.Current_Rent)
                    {
                        //    if (ex == null) ex = p => p.CurrentUse_Rent > 0;
                        //   else ex = ex.Or(p => p.CurrentUse_Rent > 0);
                    }
                    if (advanceCondition.Current_Lend)
                    {
                        //    if (ex == null) ex = p => p.CurrentUse_Lend > 0;
                        //    else ex = ex.Or(p => p.CurrentUse_Lend > 0);
                    }
                    if (advanceCondition.Current_Idle)
                    {
                        //    if (ex == null) ex = p => p.CurrentUse_Idle > 0;
                        //    else ex = ex.Or(p => p.CurrentUse_Idle > 0);
                    }

                    if (ex != null) expression = expression.And(ex);
                }
                #endregion

                if (advanceCondition.NextStep.Count != 0)
                    //   expression = expression.And(p => advanceCondition.NextStep.Contains((int)p.NextStepUsage));

                    #region 价格区间集合
                    if (advanceCondition.Price.Count > 0)
                    {
                        foreach (var range in advanceCondition.Price)
                        {
                            var min = range[0];
                            var max = range[1];

                            if (max == 0) max = int.MaxValue;

                            if (min >= max) continue;

                            //   expression = expression.And(p => p.Price >= min && p.Price <= max);
                        }
                    }
                #endregion

                #region 价格区间集合
                if (advanceCondition.Price.Count > 0)
                {
                    foreach (var range in advanceCondition.Price)
                    {
                        var min = range[0];
                        var max = range[1];

                        if (max == 0) max = int.MaxValue;

                        if (min >= max) continue;

                        //  expression = expression.And(p => p.Price >= min && p.Price <= max);
                    }
                }
                #endregion

                if (advanceCondition.LifeTime.Count == 2)
                {
                    var min = advanceCondition.LifeTime[0];
                    var max = advanceCondition.LifeTime[1];

                    //    expression = expression.And(p => p.LifeTime >= min && p.LifeTime <= max);
                }

                //if (advanceCondition.GetedDate.Count == 2)
                //{
                //    var min = advanceCondition.GetedDate[0];
                //    var max = advanceCondition.GetedDate[1];

                //    expression = expression.And(p => p.GetedDate >= new DateTime(min, 1, 1) && p.GetedDate <= new DateTime(max, 12, 31));
                //}

                //范围过滤
                if (advanceCondition.Extent != null)
                    expression = expression.And(p => !advanceCondition.Extent.Intersects(p.Location)); 
                #endregion
            }

            query = query.Where(expression);
 
            var defaultSort = new PropertySortCondition("Id", System.ComponentModel.ListSortDirection.Ascending);
            if (sortConditions != null && sortConditions.Length != 0)
            {
                query = query.Sort(sortConditions[0]);
            }
            else
            {
                query = query.Sort(new PropertySortCondition("DisplayOrder", System.ComponentModel.ListSortDirection.Ascending));
            }

            var properties = new PagedList<CSCZJ.Core.Domain.Properties.Property>(query, pageIndex, pageSize);
            return properties;
        }

        public IList<CSCZJ.Core.Domain.Properties.Property> GetAllPropertiesByDistance(double lat = 28.9721214555, double lng = 118.8898357316, string search = "", int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var sql = "declare @currentLocation geography select @currentLocation = geography::STPointFromText('POINT (114.403479 30.556776)', 4326)select *,Location.STDistance(@currentLocation) as 距离 from CSStateProperty.dbo.Property where Location.STDistance(@currentLocation) < 1000000 order by Location.STDistance(@currentLocation)";
            var query = _context.SqlQuery<CSCZJ.Core.Domain.Properties.Property>(sql);

            return query.ToList();
            //var query = from ps in _propertyRepository.TableNoTracking
            //            where !ps.Deleted
            //            select ps;

            //_propertyReposito

            //var panoramaScenes = query.ToList();

            //var panoramaScenesOrderedResult = panoramaScenes
            //    .OrderBy(ps => GeographyHelper.GetDistance(ps.PanoramaLocation.Lat, ps.PanoramaLocation.Lng, lat, lng))
            //    .ThenByDescending(ps => ps.ProductionDate);

            //var locationNameList = new List<string>();
            //var resultPanomarScenesFilterResult = new List<PanoramaScene>();
            //foreach (var scene in panoramaScenesOrderedResult)
            //{
            //    if (locationNameList.Contains(scene.PanoramaLocation.Name))
            //    {
            //        continue;
            //    }
            //    else
            //    {
            //        resultPanomarScenesFilterResult.Add(scene);
            //        locationNameList.Add(scene.PanoramaLocation.Name);
            //    }
            //}

            //var resultPanomarScenesResult = resultPanomarScenesFilterResult.Skip(pageIndex * pageSize).Take(pageSize);
            //return resultPanomarScenesResult;
        }

        public CSCZJ.Core.Domain.Properties.Property GetPropertyById(int propertyId)
        {
            if (propertyId == 0) return null;

            string key = string.Format(PROPERTY_BY_ID_KEY, propertyId);
            return _cacheManager.Get(key, () => _propertyRepository.GetById(propertyId));
        }

        public void InsertProperty(CSCZJ.Core.Domain.Properties.Property property)
        {
            if (property == null)
                throw new ArgumentNullException("property is null");

            _propertyRepository.Insert(property);

            //cache
            _cacheManager.RemoveByPattern(PROPERTIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(property);
        }

        /// <summary>
        /// 名称是否唯一
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns>true表示唯一</returns>
        public bool NameUniqueCheck(string propertyName)
        {
            var query = _propertyRepository.Table;
            query = query.Where(c => !c.Deleted);

            if (!String.IsNullOrWhiteSpace(propertyName))
            {
                var property = query.Where(c => c.Name == propertyName).FirstOrDefault();
                return property == null;
            }
            else return true;
        }

        public void UpdateProperty(CSCZJ.Core.Domain.Properties.Property property)
        {
            if (property == null)
                throw new ArgumentNullException("property is null");

            _propertyRepository.Update(property);

            //cache
            _cacheManager.RemoveByPattern(PROPERTIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(property);
        }

        public IList<Core.Domain.Properties.Property> GetPropertyProcess(int governmentId)
        {
            var query = from c in _propertyRepository.Table
                        where c.Government.Id == governmentId && c.Deleted == false && c.Locked == false//&&c.Published==true
                        select c;
            var properties = query.ToList();
            return properties;
        }

        /// <summary>
        /// 获取可处理的资产
        /// </summary>
        /// <param name="governmentIds"></param>
        /// <returns></returns>
        public IList<CSCZJ.Core.Domain.Properties.Property> GetProcessProperties(string name,IList<int> governmentIds)
        {
            var query = from p in _propertyRepository.TableNoTracking
                        where !p.Deleted && !p.Locked && p.Published && p.Name.Contains(name)// governmentIds.Contains(p.Government.Id)
                        orderby p.CreatedOn descending
                        select p;

            return query.ToList();
        }

        public IList<Core.Domain.Properties.Property> GetAllProcessProperties(IList<int> governmentIds, string search = "", PropertyAdvanceConditionRequest advanceCondition = null,  params PropertySortCondition[] sortConditions)
        {
            var query = _propertyRepository.Table.AsNoTracking();

            query = query.Where(p => !p.Deleted && governmentIds.Contains(p.Government.Id));

            Expression<Func<CSCZJ.Core.Domain.Properties.Property, bool>> expression = p => !p.Deleted && !p.Locked && p.Published ;
             

            //字符串查询
            if (!string.IsNullOrEmpty(search))
            {
                expression = expression.And(p => p.Name.Contains(search) || p.Address.Contains(search));
                //query = query.Where(e => e.Name.Contains(search) || e.Address.Contains(search));
            }

            //高级搜索条件
            if (advanceCondition != null) 
            {
                if (advanceCondition.GovernmentId > 0)
                {
                    if (advanceCondition.SerachParentGovernement)
                    {
                        var ids = _governmentUnitRepository.TableNoTracking.Where(g => g.ParentGovernmentId == advanceCondition.GovernmentId || g.Id == advanceCondition.GovernmentId).Select(g => g.Id).ToArray();

                        expression = expression.And(p => ids.Contains(p.Government.Id));
                    }
                    else
                    {
                        expression = expression.And(p => p.Government.Id == advanceCondition.GovernmentId);
                    }
                }

                if (advanceCondition.GovernmentTypes.Count != 0)
                    expression = expression.And(p => advanceCondition.GovernmentTypes.Contains((int)p.Government.GovernmentType));
                //query = query.Where(s => advanceCondition.PropertyType.Contains((int)s.Government.GovernmentType));

                #region 资产类别和建筑面积、土地面积挂钩
                Expression<Func<CSCZJ.Core.Domain.Properties.Property, bool>> houseAndLandExpression = null;

                #region 建筑面积区间集合
                Expression<Func<CSCZJ.Core.Domain.Properties.Property, bool>> constructRangeExpression = null;
                if (advanceCondition.ConstructArea.Count > 0)
                {
                    #region 遍历区间集合
                    foreach (var range in advanceCondition.ConstructArea)
                    {
                        var min = range[0];
                        var max = range[1];

                        if (max == 0) max = int.MaxValue;

                        if (min >= max) continue;

                        if (constructRangeExpression == null) constructRangeExpression = p => p.ConstructArea >= min && p.ConstructArea <= max;
                        else constructRangeExpression = constructRangeExpression.Or(p => p.ConstructArea >= min && p.ConstructArea <= max);
                    }
                    #endregion                 
                }

                #endregion

                #region 土地面积区间集合
                Expression<Func<CSCZJ.Core.Domain.Properties.Property, bool>> landRangeExpression = null;

                if (advanceCondition.LandArea.Count > 0)
                {
                    #region 遍历区间集合
                    foreach (var range in advanceCondition.LandArea)
                    {
                        var min = range[0];
                        var max = range[1];

                        if (max == 0) max = int.MaxValue;

                        if (min >= max) continue;

                        if (landRangeExpression == null) landRangeExpression = p => p.LandArea >= min && p.LandArea <= max;
                        else landRangeExpression = landRangeExpression.Or(p => p.LandArea >= min && p.LandArea <= max);
                    }
                    #endregion                 
                }
                #endregion

                if (advanceCondition.PropertyType.Count == 0)
                {
                    if (landRangeExpression != null)
                    {
                        if (houseAndLandExpression == null) houseAndLandExpression = landRangeExpression;
                        else houseAndLandExpression = houseAndLandExpression.Or(landRangeExpression);
                    }
                }
                else
                {

                    #region 房屋及房屋面积、土地面积
                    if (advanceCondition.PropertyType.Contains(0))
                    {
                        Expression<Func<CSCZJ.Core.Domain.Properties.Property, bool>> xpression = null;//p => p.PropertyType == CSCZJ.Core.Domain.Properties.PropertyType.House;

                        if (constructRangeExpression != null)
                            xpression = xpression.And(constructRangeExpression);

                        if (landRangeExpression != null)
                            xpression = xpression.And(landRangeExpression);

                        houseAndLandExpression = xpression;
                    }
                    #endregion

                    #region 土地及土地面积
                    if (advanceCondition.PropertyType.Contains(1) || advanceCondition.PropertyType.Contains(2))
                    {
                        if (advanceCondition.PropertyType.Contains(0)) advanceCondition.PropertyType.Remove(0);
                        Expression<Func<CSCZJ.Core.Domain.Properties.Property, bool>> xpression = p => advanceCondition.PropertyType.Contains((int)p.PropertyType);

                        if (landRangeExpression != null)
                            xpression = xpression.And(landRangeExpression);

                        if (houseAndLandExpression == null) houseAndLandExpression = xpression;
                        else houseAndLandExpression = houseAndLandExpression.Or(xpression);
                    }
                    #endregion
                }

                if (houseAndLandExpression != null) expression = expression.And(houseAndLandExpression);
                #endregion

                if (advanceCondition.Region.Count != 0)
                    expression = expression.And(p => advanceCondition.Region.Contains((int)p.Region));

                #region 土地证书情况
                if (!(advanceCondition.Certificate_Both && advanceCondition.Certificate_Construct &&
                            advanceCondition.Certificate_Land && advanceCondition.Certificate_None))
                {
                    Expression<Func<CSCZJ.Core.Domain.Properties.Property, bool>> ex = null;
               //     if (advanceCondition.Certificate_Both) ex = p => p.HasConstructID && p.HasLandID;
                    if (advanceCondition.Certificate_Construct)
                    {
                    //    if (ex == null) ex = p => p.HasConstructID && !p.HasLandID;
                     //   else ex = ex.Or(p => p.HasConstructID && !p.HasLandID);
                    }
                    if (advanceCondition.Certificate_Land)
                    {
                     //   if (ex == null) ex = p => !p.HasConstructID && p.HasLandID;
                       // else ex = ex.Or(p => !p.HasConstructID && p.HasLandID);
                    }
                    if (advanceCondition.Certificate_None)
                    {
                      //  if (ex == null) ex = p => !p.HasConstructID && !p.HasLandID;
                       // else ex = ex.Or(p => !p.HasConstructID && !p.HasLandID);
                    }

                    if (ex != null) expression = expression.And(ex);
                }
                #endregion

                #region 使用现状
                if (!(advanceCondition.Current_Self && advanceCondition.Current_Rent && advanceCondition.Current_Lend && advanceCondition.Current_Idle))
                {
                    Expression<Func<CSCZJ.Core.Domain.Properties.Property, bool>> ex = null;
                 //   if (advanceCondition.Current_Self) ex = p => p.CurrentUse_Self > 0;
                    if (advanceCondition.Current_Rent)
                    {
                    //    if (ex == null) ex = p => p.CurrentUse_Rent > 0;
                     //   else ex = ex.Or(p => p.CurrentUse_Rent > 0);
                    }
                    if (advanceCondition.Current_Lend)
                    {
                      //  if (ex == null) ex = p => p.CurrentUse_Lend > 0;
                      //  else ex = ex.Or(p => p.CurrentUse_Lend > 0);
                    }
                    if (advanceCondition.Current_Idle)
                    {
                    //    if (ex == null) ex = p => p.CurrentUse_Idle > 0;
                     //   else ex = ex.Or(p => p.CurrentUse_Idle > 0);
                    }

                    if (ex != null) expression = expression.And(ex);
                }
                #endregion

                if (advanceCondition.NextStep.Count != 0)
              //      expression = expression.And(p => advanceCondition.NextStep.Contains((int)p.NextStepUsage));

                #region 价格区间集合
                if (advanceCondition.Price.Count > 0)
                {
                    foreach (var range in advanceCondition.Price)
                    {
                        var min = range[0];
                        var max = range[1];

                        if (max == 0) max = int.MaxValue;

                        if (min >= max) continue;

                     //   expression = expression.And(p => p.Price >= min && p.Price <= max);
                    }
                }
                #endregion

                #region 价格区间集合
                if (advanceCondition.Price.Count > 0)
                {
                    foreach (var range in advanceCondition.Price)
                    {
                        var min = range[0];
                        var max = range[1];

                        if (max == 0) max = int.MaxValue;

                        if (min >= max) continue;

                     //   expression = expression.And(p => p.Price >= min && p.Price <= max);
                    }
                }
                #endregion

                if (advanceCondition.LifeTime.Count == 2)
                {
                    var min = advanceCondition.LifeTime[0];
                    var max = advanceCondition.LifeTime[1];

                 //   expression = expression.And(p => p.LifeTime >= min && p.LifeTime <= max);
                }

                //if (advanceCondition.GetedDate.Count == 2)
                //{
                //    var min = advanceCondition.GetedDate[0];
                //    var max = advanceCondition.GetedDate[1];

                //    expression = expression.And(p => p.GetedDate >= new DateTime(min, 1, 1) && p.GetedDate <= new DateTime(max, 12, 31));
                //}

                //范围过滤
                if (advanceCondition.Extent != null)
                    expression = expression.And(p => !advanceCondition.Extent.Intersects(p.Location));
            }

            query = query.Where(expression);

            if (sortConditions != null && sortConditions.Length != 0)
            {
                query = query.Sort(sortConditions);
            }
            else
            {
                query = query.Sort(new PropertySortCondition[1] {
                    new PropertySortCondition("DisplayOrder", System.ComponentModel.ListSortDirection.Ascending)
                });
            }

            var properties = query.ToList();
            return properties;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberId"></param>
        /// <param name="typeId">证件类型，0为不动产证 1为房产证</param>
        /// <returns></returns>
        public IList<Core.Domain.Properties.Property> GetPropertiesBySameNumberId(string numberId, string typeId)
        {
            var query = _propertyRepository.TableNoTracking;

            query = query.Where(p => !p.Deleted);


            if(typeId=="0")
            {
                query = query.Where(p => p.EstateId == numberId);
            }
            else if (typeId == "1")
            {
                query = query.Where(p => p.PropertyID == numberId);
            }

            query = query.OrderBy(p => p.ParentPropertyId).ThenBy(p => p.Id);


            return query.ToList();
        }


        public void DeletePropertyPicture(PropertyPicture propertyPicture)
        {
            if (propertyPicture == null)
                throw new ArgumentNullException("propertyPicture");

            _propertyPictureRepository.Delete(propertyPicture);

            //event notification
            _eventPublisher.EntityDeleted(propertyPicture);
        }

        public IList<PropertyPicture> GetPropertyPicturesByPropertyId(int propertyId)
        {
            var query = from sp in _propertyPictureRepository.Table
                        where sp.PropertyId == propertyId
                        orderby sp.DisplayOrder
                        select sp;

            var propertyPictures = query.ToList();
            return propertyPictures;
        }

        public PropertyPicture GetPropertyPictureById(int propertyPictureId)
        {
            if (propertyPictureId == 0)
                return null;

            return _propertyPictureRepository.GetById(propertyPictureId);
        }

        public void InsertPropertyPicture(PropertyPicture propertyPicture)
        {
            if (propertyPicture == null)
                throw new ArgumentNullException("propertyPicture");

            _propertyPictureRepository.Insert(propertyPicture);

            //event notification
            _eventPublisher.EntityInserted(propertyPicture);
        }

        public void UpdatePropertyPicture(PropertyPicture propertyPicture)
        {
            if (propertyPicture == null)
                throw new ArgumentNullException("propertyPicture");

            _propertyPictureRepository.Update(propertyPicture);

            //event notification
            _eventPublisher.EntityUpdated(propertyPicture);
        }

        public void DeletePropertyFile(PropertyFile propertyFile)
        {
            if (propertyFile == null)
                throw new ArgumentNullException("propertyFile");

            _propertyFileRepository.Delete(propertyFile);

            //event notification
            _eventPublisher.EntityDeleted(propertyFile);
        }

        public IList<PropertyFile> GetPropertyFilesByPropertyId(int propertyId)
        {
            var query = from sp in _propertyFileRepository.Table
                        where sp.PropertyId == propertyId
                        orderby sp.DisplayOrder
                        select sp;

            var propertyFiles = query.ToList();
            return propertyFiles;
        }

        public PropertyFile GetPropertyFileById(int propertyFileId)
        {
            if (propertyFileId == 0)
                return null;

            return _propertyFileRepository.GetById(propertyFileId);
        }

        public void InsertPropertyFile(PropertyFile propertyFile)
        {
            if (propertyFile == null)
                throw new ArgumentNullException("propertyFile");

            _propertyFileRepository.Insert(propertyFile);

            //event notification
            _eventPublisher.EntityInserted(propertyFile);
        }

        public void UpdatePropertyFile(PropertyFile propertyFile)
        {
            if (propertyFile == null)
                throw new ArgumentNullException("propertyFile");

            _propertyFileRepository.Update(propertyFile);

            //event notification
            _eventPublisher.EntityUpdated(propertyFile);
        }

        public IList<Core.Domain.Properties.Property> GetPropertiesByGId(int id)
        {
            var quety = from c in _propertyRepository.Table
                        where c.Government.Id == id
                        select c;
            var properties = quety.ToList();
            return properties;
        }

        public IList<Core.Domain.Properties.Property> GetCurrentGovermentProperties(string name)
        {
            var query = from c in _propertyRepository.Table
                        where c.Government.Name == name
                        select c;
            var properties = query.ToList();
            return properties;
        }

        public Region GetPropertyRegion(DbGeography location)
        {
            var xq = DbGeography.FromText(xqregion);
            var kc = DbGeography.FromText(kcegion);
            var qj = DbGeography.FromText(qjregion);
            var lcq = DbGeography.FromText(lcqregion);
            var jjq = DbGeography.FromText(jjqregion);

            //if (xq.Intersects(location)) return Region.West;
            //else if (kc.Intersects(location)) return Region.KC;
            //else if (qj.Intersects(location)) return Region.QJ;
            //else if (jjq.Intersects(location)) return Region.Clusters;
            //else if (lcq.Intersects(location)) return Region.OldCity;
             return Region.Others;
        }

        public List<Core.Domain.Properties.Property> GetExportMonthTotalProperties(int id)
        {

            if (id == 37)
            {
                var query = from c in _propertyRepository.Table
                            where (c.Government.GovernmentType == GovernmentType.Government || c.Government.GovernmentType == GovernmentType.Institution) && c.Deleted == false
                            select c;
                var properties = query.ToList();
                return properties;
            }
            else if (id == 26)
            {
                var query = from c in _propertyRepository.Table
                            where (c.Government.GovernmentType == GovernmentType.Company || c.Government.GovernmentType == GovernmentType.Group) && c.Deleted == false
                            select c;
                var properties = query.ToList();
                return properties;
            }
            else {
                return null;
            }
            
           
        }

        public IList<Core.Domain.Properties.Property> GetMonthTotalPropertyProcess(int governmentId)
        {
            var query = from c in _propertyRepository.Table
                        where c.Government.Id == governmentId && c.Published == true && c.Deleted == false
                        select c;
            return query.ToList();

        }

        public IList<Core.Domain.Properties.Property> GetKeyProperties(string search)
        {
            var query = from c in _propertyRepository.Table
                        where c.Name.Contains(search) || c.Address.Contains(search)
                        select c;
            return query.ToList();

          //  throw new NotImplementedException();
        }

        public IList<Core.Domain.Properties.Property> GetHighSearchProperties(ArrayList properyTypeList, IList<int> regionList, ArrayList areaList, IList<int> currentList, ArrayList rightList)
        {
            var query = _propertyRepository.Table.AsNoTracking().AsQueryable();

            Expression<Func<CSCZJ.Core.Domain.Properties.Property, bool>> expression = p => !p.Deleted;
            #region 资产类别和建筑面积土地面积

            if (properyTypeList.Count != 1)
            {
                if (areaList.Count >1)
                {
                    int min = 0, max = 10000000;
                        min = Convert.ToInt32(areaList[0]);
                        max = Convert.ToInt32(areaList[areaList.Count - 1]);
                    if (min == 49 && max != 1001)
                    {
                        expression = expression.And(p => p.ConstructArea <= max);
                        expression = expression.Or(p => p.LandArea <= max);
                    }
                    else if (max == 1001 && min != 49)
                    {
                        expression = expression.And(p => p.ConstructArea >= min);
                        expression = expression.Or(p => p.LandArea >= min);
                    }
                    else if (min != 49 && max != 1001) {
                        expression = expression.And(p => p.ConstructArea >= min && p.ConstructArea <= max);
                        expression = expression.Or(p => p.LandArea >= min && p.LandArea <= max);
                    } 
                }

                else if (areaList.Count == 1) {
                    switch ((int)areaList[0]) {
                        case 49:
                            expression = expression.And(p => p.ConstructArea <50);
                            expression = expression.Or(p => p.LandArea < 50);
                            break;
                        case 1001:
                            expression = expression.And(p => p.ConstructArea >1000);
                            expression = expression.Or(p => p.LandArea > 1000);
                            break;
                    }               
                }


            }
            else  if(properyTypeList.Count==1){

                if (properyTypeList.Contains(0)) {

                    if (areaList.Count > 1)
                    {                      
                        int min = 0, max = 10000000;
                            min =Convert.ToInt32( areaList[0]);
                            max = Convert.ToInt32( areaList[areaList.Count - 1]);
                        if (min == 49 &&max!=1001) expression = expression.And(p => p.ConstructArea <= max);
                        else if(max==1001&&min!=49) expression = expression.And(p => p.ConstructArea >=min);
                        else if(min!=49&&max!=1001) expression = expression.And(p => p.ConstructArea >= min&&p.ConstructArea<=max);
                    }

                    else if (areaList.Count == 1)
                    {
                        switch ((int)areaList[0])
                        {
                            case 49:
                                expression = expression.And(p => p.ConstructArea < 50);
                              //  expression = expression.And(p => p.PropertyType == PropertyType.House);
                                break;
                            case 1001:
                                expression = expression.And(p => p.ConstructArea >= 1000);
                             //   expression = expression.And(p => p.PropertyType == PropertyType.House);
                                break;
                        }
                    }
                  //  expression = expression.And(p => p.PropertyType == PropertyType.House);
                }

                if (properyTypeList.Contains(1))
                {

                    if (areaList.Count > 1)
                    {
                            int min = 0, max = 10000000;
                            min = Convert.ToInt32(areaList[0]);
                            max = Convert.ToInt32(areaList[areaList.Count - 1]);

                        if (min == 49 && max != 1001) expression = expression.And(p => p.LandArea <= max);
                        else if (max == 1001 && min != 49) expression = expression.And(p => p.LandArea >= min);
                        else if (min != 49 && max != 1001) expression = expression.And(p => p.LandArea >= min && p.ConstructArea <= max);
                    }

                    else if (areaList.Count == 1)
                    {
                        switch ((int)areaList[0])
                        {
                            case 50:
                                expression = expression.And(p => p.LandArea <= 50);
                            //    expression = expression.And(p => p.PropertyType == PropertyType.Land);
                                break;
                            case 1001:
                                expression = expression.And(p => p.LandArea >= 1000);
                           //     expression = expression.And(p => p.PropertyType == PropertyType.Land);
                                break;
                        }
                    }
                    else
                    {
                      //  expression = expression.And(p => p.PropertyType == PropertyType.Land);
                    }
                }

            }
            #endregion

            if (regionList.Count != 0)  expression = expression.And(p => regionList.Contains((int)p.Region));

            if (currentList.Count != 0) expression = expression.And(p => currentList.Contains((int)p.CurrentType));

            if (rightList.Count != 0) {
                if(rightList.Contains(1)) expression = expression.Or(p => p.LandId!=null);
                if (rightList.Contains(2)) expression = expression.Or(p => p.ConstructId != null);
                if (rightList.Contains(3)) expression = expression.Or(p => p.LandId != null && p.ConstructId!=null);
                if (rightList.Contains(0)) expression = expression.Or(p => p.LandId == null&&p.ConstructId==null);
            }

            query = query.Where(expression);

            return query.ToList();
        }
    }
}