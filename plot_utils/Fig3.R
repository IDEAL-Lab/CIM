library(ggplot2)
library(reshape2)
library(grid)

# set workspace
this.dir <- dirname(parent.frame(2)$ofile)
setwd(this.dir)
dirnames <- c('wiki-Vote', 'ca-AstroPh', 'com-dblp', 'com-lj')
discounts <- c('0.7', '0.85', '1')
yhighs <- c(500, 600, 800, 1000, 1800, 3000, 3000, 4500, 7000, 24000, 40000, 120000)
ysteps <- c(100, 100, 100, 100, 200, 200, 500, 500, 500, 1000, 4000, 10000, 20000)

genfic <- function(dirname, discount, y_high, y_step) {
  # main procedure
  loc <- paste('./evaluation(0.85,0.05)/', dirname, sep='')
  
  fres <- paste(loc, '/Alpha=', discount, '/Fig3.txt', sep = '')
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
    scale_y_continuous(breaks=seq(0, y_high, y_step)) +
    #   scale_x_continuous(breaks=seq(0,50,5)) +
    xlab("Budget") +
    ylab("Influence Spread") +
    theme(
      legend.position = c(0.09, 0.85), # c(0,0) bottom left, c(1,1) top-right.
      legend.title=element_blank(),
      # legend.key.width=unit(0.6, "cm"),
      legend.text=element_text(size=14),
      axis.text=element_text(size=12),
      axis.title=element_text(size=18),
      plot.title=element_text(size=18)
    ) +
    ggtitle(bquote(.(dirname) ~ 'with' ~ alpha ~ '=' ~ .(discount)))
  
  figloc <- paste('./eval/influ_', 
                  dirname, '_', discount, '.pdf', sep='')
  ggsave(file=figloc, width = 12, height = 8, units = "in")
}

print('Generating Influence plots...')
for (i in 1:4) {
  for (j in 1:3) {
    ind <- (i-1) * 3 + j
    print(ind)
    genfic(dirnames[i], discounts[j], yhighs[ind], ysteps[ind])
  }
}
    
