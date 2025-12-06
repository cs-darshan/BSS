using System;
using System.Collections.Generic;

namespace Sensore.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public UserRole Role { get; set; }  // Patient, Clinician, Admin
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }

        public List<PressureFrame> PressureFrames { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
        public List<ClinicianPatient> AssignedPatients { get; set; } = new();
    }

    public enum UserRole
    {
        Patient = 0,
        Clinician = 1,
        Admin = 2
    }

    public class PressureFrame
    {
        public int FrameId { get; set; }
        public int UserId { get; set; }
        public DateTime Timestamp { get; set; }
        public string PressureMatrix { get; set; } = null!; // JSON 32x32

        public int PeakPressure { get; set; }
        public double ContactAreaPercent { get; set; }
        public double RiskScore { get; set; }
        public bool IsAlerted { get; set; }
        public string? AlertMessage { get; set; }

        public User User { get; set; } = null!;
        public List<Comment> Comments { get; set; } = new();
    }

    public class Metric
    {
        public int MetricId { get; set; }
        public int FrameId { get; set; }
        public DateTime Timestamp { get; set; }
        public int PeakPressureIndex { get; set; }
        public double ContactAreaPercent { get; set; }
        public double RiskScore { get; set; }
        public int PixelsAboveThreshold { get; set; }

        public PressureFrame PressureFrame { get; set; } = null!;
    }

    public class Comment
    {
        public int CommentId { get; set; }
        public int FrameId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public int? ParentCommentId { get; set; }

        public PressureFrame PressureFrame { get; set; } = null!;
        public User User { get; set; } = null!;
        public Comment? ParentComment { get; set; }
        public List<Comment> Replies { get; set; } = new();
    }

    public class AlertLog
    {
        public int AlertId { get; set; }
        public int FrameId { get; set; }
        public int UserId { get; set; }
        public DateTime AlertDate { get; set; }
        public string AlertType { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsReviewed { get; set; }
        public int? ReviewedByClinicianId { get; set; }

        public PressureFrame PressureFrame { get; set; } = null!;
        public User User { get; set; } = null!;
    }

    public class ClinicianPatient
    {
        public int Id { get; set; }
        public int ClinicianId { get; set; }
        public int PatientId { get; set; }
        public DateTime AssignedDate { get; set; }

        public User Clinician { get; set; } = null!;
        public User Patient { get; set; } = null!;
    }
}
