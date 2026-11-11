# Plot Harvest

Compare two treatments within replicate blocks and retain incomplete pairs.

## Try it

From this folder with the .NET 10 SDK installed:

```sh
dotnet run -- trials study.tsv
```

Rows are plot ID, block, treatment, area in m² and harvest kg. Exactly two named treatments form a contrast. Repeated plots within a treatment and block are area-weighted. Differences are second minus first in ordinal treatment order.

The included study.tsv uses tab-separated fields. Comment lines start with `#`.

The original exercise remains available with `dotnet run -- example.tsv`.

## When to use it

An agronomy student compares varieties grown in unequal trial plots.

Example result:

```text
P1	North	A	3000 kg/ha
P2	North	B	4000 kg/ha
North: B minus A, difference +1000 kg/ha
P3	South	A	2000 kg/ha
South: incomplete pair, missing B
Mean within-block difference 1000 kg/ha (1 complete blocks)
```

Related course or field workflow: [University of Florida on field trial design](https://ask.ifas.ufl.edu/publication/AG447).
