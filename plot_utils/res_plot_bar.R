library(ggplot2)
library(reshape2)
library(grid)

# set workspace
dirnames <- c('wiki-Vote', 'ca-AstroPh', 'com-dblp', 'com-lj')
discounts <- c('0.7', '0.85', '1')

# input
dirselect <- readline("Enter 1..4 to choose dataset: ")
disselect <- readline("Enter 1..3 to choose discount: ")
dirname <- dirnames[as.integer(dirselect)]
discount <- discounts[as.integer(disselect)]

# main procedure
loc <- paste('~/Desktop/results/evaluation/', dirname, sep='')

fres <- paste(loc, '/Alpha=', discount, '/AllResultsFormat.txt', sep = '')
dres <- read.csv(fres, sep=' ', stringsAsFactors=F, strip.white=TRUE)

dres$M <- factor(dres$M, levels = c('CD', 'UC', 'IM'))
ggplot(dres, aes(x=B, y=INFLU, fill=M, color=M)) +
  # scale_fill_manual(values=c("#CC6666", "#9999CC", "#66CC99")) +
  # scale_fill_manual(values=c('#6E548D', '#DB843D', '#C0504D')) + 
  scale_fill_brewer() +
  theme_bw() +
  geom_bar(position=position_dodge(), stat='identity', alpha=1) + 
  geom_errorbar(aes(ymin=INFLU-STD, ymax=INFLU+STD),
                width=2.5,                    # width of the error bars
                position=position_dodge(9)) + 
  scale_y_continuous(breaks=seq(0,120000,20000)) +
#   scale_x_continuous(breaks=seq(0,50,5)) +
  xlab("Budget") +
  ylab("Influence Spread") +
  theme(
    # legend.position = c(0.8, 0.2), # c(0,0) bottom left, c(1,1) top-right.
    legend.title=element_blank(),
    # legend.key.width=unit(0.6, "cm"),
    legend.text=element_text(size=9)
  ) +
  ggtitle(bquote(.(dirname) ~ 'with' ~ alpha ~ '=' ~ .(discount)))

figloc <- paste('~/Desktop/results/eval/influ_', 
                dirname, '_', discount, '.eps', sep='')
ggsave(file=figloc)