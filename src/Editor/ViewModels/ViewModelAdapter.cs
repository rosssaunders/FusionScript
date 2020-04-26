using RxdSolutions.FusionScript.Model;
using RxdSolutions.FusionScript.ViewModels;
using RxdSolutions.FusionScript.Client;

namespace RxdSolutions.FusionScript.Adapters
{
    public class ViewModelAdapter
    {
        private readonly DataServiceClient _factory;

        public ViewModelAdapter(DataServiceClient factory)
        {
            _factory = factory;
        }

        public ScriptViewModel Adapt(ScriptModel model)
        {
            var vm = new ScriptViewModel(_factory)
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Script = model.Script,
                Language = model.Language,
                OwnerId = model.OwnerId,
                SecurityPermission = model.SecurityPermission,
                Icon = model.Icon
            };

            foreach(var trigger in model.Triggers)
            {
                var vmt = new TriggerViewModel()
                {
                    Trigger = trigger.Trigger,
                    Id = trigger.Id,
                    Time = trigger.Time
                };

                vm.Triggers.Add(vmt);
            }

            vm.Validate();

            return vm;
        }

        public void Update(ScriptModel Model, ScriptViewModel existingItem)
        {
            existingItem.Name = Model.Name;
            existingItem.Description = Model.Description;
            existingItem.Language = Model.Language;
            
            existingItem.OwnerId = Model.OwnerId;
            existingItem.SecurityPermission = Model.SecurityPermission;
            existingItem.Icon = Model.Icon;
            existingItem.Script = Model.Script;
            existingItem.Triggers.Clear();

            foreach (var trigger in Model.Triggers)
            {
                var vmt = new TriggerViewModel()
                {
                    Trigger = trigger.Trigger,
                    Id = trigger.Id,
                    Time = trigger.Time
                };

                existingItem.Triggers.Add(vmt);
            }

            existingItem.Validate();
        }

        public ScriptModel Adapt(ScriptViewModel model)
        {
            var fm = new ScriptModel()
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Script = model.Script,
                Language = model.Language,
                OwnerId = model.OwnerId,
                SecurityPermission = model.SecurityPermission,
                Icon = model.Icon
            };

            foreach(var trigger in model.Triggers)
            {
                fm.Triggers.Add(new ScriptTriggerModel()
                {
                    ScriptId = model.Id,
                    Id = trigger.Id,
                    Trigger = trigger.Trigger,
                    Time = trigger.Time
                });
            }

            return fm;
        }
    }
}
