    private RotateRigidbodyAroundPointBy(rb : Rigidbody, origin:Vector3, axis : Vector3, angle:number) {
        let q : Quaternion = Quaternion.AngleAxis(angle, axis);
        rb.MovePosition(q * (rb.transform.position - origin) + origin);
        rb.MoveRotation(rb.transform.rotation * q);
    }
