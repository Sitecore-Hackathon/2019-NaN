using System;
using System.Linq.Expressions;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Facets;
using Hackathon.Boilerplate.Foundation.BusinessValueTracker.Models.Clients;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Framework.Rules;
using Sitecore.XConnect;
using Sitecore.XConnect.Segmentation.Predicates;

namespace Hackathon.Boilerplate.Foundation.BusinessValueTracker.Conditionals
{
    public class RfmMatch : ICondition, IContactSearchQueryFactory
    {
        public Guid RfmId { get; set; }
        public bool Evaluate(IRuleExecutionContext context)
        {
            var contact = context.Fact<Contact>();
            var facet = contact.GetFacet<RfmContactFacet>(RfmContactFacet.DefaultFacetKey);
            if (facet == null) return false;

            var rfm = GetRfm();
            if (rfm == null) return false;

            return facet.R == rfm.R && facet.F == rfm.F && facet.M == rfm.M;
        }

        public Expression<Func<Contact, bool>> CreateContactSearchQuery(IContactSearchQueryContext context)
        {
            var rfm = GetRfm();
            if (rfm == null) return x => false;

            return contact => contact.GetFacet<RfmContactFacet>(RfmContactFacet.DefaultFacetKey).R == rfm.R
                              && contact.GetFacet<RfmContactFacet>(RfmContactFacet.DefaultFacetKey).F == rfm.F
                              && contact.GetFacet<RfmContactFacet>(RfmContactFacet.DefaultFacetKey).M == rfm.M;
        }

        public CustomerBusinessValue GetRfm()
        {
            int r, f, m;
            Item rfmPattern = Database.GetDatabase("master").GetItem(new ID(RfmId));
            if (rfmPattern == null) return null;

            if (int.TryParse(rfmPattern["R"], out r))
            {
                if (int.TryParse(rfmPattern["F"], out f))
                {
                    if (int.TryParse(rfmPattern["M"], out m))
                    {
                        return new CustomerBusinessValue
                        {
                            R = r,
                            F = f,
                            M = m
                        };
                    }
                }
            }

            return null;
        }
    }
}