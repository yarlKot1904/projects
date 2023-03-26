#pragma once

#ifndef ISOTR
#define ISOTR
#include "base-types.h"
#include "Shape.h"


class Isotrapezium : public Shape {
public:
	Isotrapezium(point_t center, double maxWidth, double minWidth, double height) : center_(point_t(center)), firstWidth_(maxWidth), secondWidth_(minWidth), height_(height) { max = firstWidth_ > secondWidth_ ? firstWidth_ : secondWidth_; };
	Isotrapezium(double left_x, double left_y, double minWidth, double maxWidth, double height) : center_(point_t((left_x + (maxWidth > minWidth ? maxWidth : minWidth) / 2), (left_y + height / 2))), firstWidth_(maxWidth), secondWidth_(minWidth), height_(height), max(maxWidth > minWidth ? maxWidth : minWidth) {};
	Isotrapezium(const Isotrapezium& source) : center_(point_t(source.center_)), firstWidth_(source.firstWidth_), secondWidth_(source.secondWidth_), height_(source.height_) { max = firstWidth_ > secondWidth_ ? firstWidth_ : secondWidth_; };
	~Isotrapezium() = default;

	double getArea();
	Isotrapezium& clone();
	rectangle& getFrameRect();
	void move(double delta_x, double delta_y);
	void moveTo(double x, double y);
	void scale(double x, double y, double sigma);
	std::string getName();
	double getMaxWidth();
	double getMaxHeigth();
	double getPoints(int index);
	double getX() { return center_.getX(); };
	double getY() { return center_.getY(); };
private:
	point_t center_;
	double firstWidth_;
	double secondWidth_;
	double height_;
	double max;
};

#endif // !ISOTR

