library(dplyr)
library(readr)
library(ggplot2)
library(plot3D)
library(plot3Drgl)

natsim_data <- read.table(file=file.choose(),
                         header=F, sep="/",
                         na.strings="null",
                         stringsAsFactors=F)

natsim_data <- natsim_data %>%
  rename(gen=V1, id=V2, speed=V3, awareness=V4)

dist_data <- natsim_data %>%
  select(-gen) %>%
  group_by(id) %>%
  mutate(lifespan = n())
dist_data <- distinct(dist_data, id, .keep_all=T)

vis <- ggplot(data = dist_data, aes(x=awareness, y=speed)) + geom_point(aes(color=id)) + geom_smooth()
vis

lvis <- ggplot(data = dist_data, aes(x=speed, y=lifespan)) + geom_point(aes(color=id)) + geom_smooth()
lvis

ldvis <- ggplot(data = dist_data, aes(x=awareness, y=lifespan)) + geom_point(aes(color=id)) + geom_smooth()
ldvis

scatter3D(dist_data$awareness,dist_data$speed,dist_data$lifespan, phi = 30, theta = 45, ticktype = 'detailed', type='h', xlab='awareness',ylab='speed',zlab='lifespan')
plotrgl()
